using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealtorApi.Domain.Properties;
using RealtorApi.Infrastructure.Handlers;
using RealtorApi.Infrastructure.Persistence;
using RealtorApi.Infrastructure.Results;

namespace RealtorApi.Features.Properties.UpdateProperty;

public sealed class UpdatePropertyHandler : IHandler
{
    private const long MaxImageSizeBytes = 5L * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png",
        ".jpg",
        ".jpeg"
    };

    private readonly AppDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<UpdatePropertyHandler> _logger;

    public UpdatePropertyHandler(
        AppDbContext dbContext,
        IWebHostEnvironment environment,
        ILogger<UpdatePropertyHandler> logger)
    {
        _dbContext = dbContext;
        _environment = environment;
        _logger = logger;
    }

    public async Task<Result<Property>> HandleAsync(Guid id, UpdatePropertyRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var property = await _dbContext.Properties
            .FirstOrDefaultAsync(currentProperty => currentProperty.Id == id, cancellationToken);

        if (property is null)
        {
            return Result.Failure<Property>(
                code: "properties.not_found",
                message: "Property not found.",
                statusCode: StatusCodes.Status404NotFound,
                detail: $"Property '{id}' was not found.");
        }

        var oldImageUrl = property.ImageUrl;
        var updatedAt = DateTime.UtcNow;
        string? newStoredImagePath = null;
        string? newImageUrl = oldImageUrl;

        try
        {
            if (request.ImageFile is not null)
            {
                var imageValidation = await ValidateImageFileAsync(request.ImageFile, cancellationToken);
                if (imageValidation is not null)
                {
                    return imageValidation;
                }

                var propertiesDirectory = GetPropertiesDirectory();
                Directory.CreateDirectory(propertiesDirectory);

                var extension = Path.GetExtension(request.ImageFile.FileName);
                var storedFileName = $"{Guid.NewGuid():N}{extension}";
                newStoredImagePath = Path.Combine(propertiesDirectory, storedFileName);

                await using (var fileStream = File.Create(newStoredImagePath))
                {
                    await request.ImageFile.CopyToAsync(fileStream, cancellationToken);
                }

                newImageUrl = UpdatePropertyMapping.BuildPublicImageUrl(storedFileName);
            }

            property.Title = request.Title!.Trim();
            property.Description = request.Description!.Trim();
            property.Address = request.Address!.Trim();
            property.Price = request.Price!.Value;
            property.Status = request.Status!.Value;
            property.BedroomCount = request.BedroomCount!.Value;
            property.BathroomCount = request.BathroomCount!.Value;
            property.AreaSquareMeters = request.AreaSquareMeters!.Value;
            property.ImageUrl = newImageUrl;
            property.UpdatedAt = updatedAt;

            await _dbContext.SaveChangesAsync(cancellationToken);

            if (request.ImageFile is not null && !string.IsNullOrWhiteSpace(oldImageUrl))
            {
                TryDeleteStoredImage(oldImageUrl);
            }

            return Result.Success(property);
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrWhiteSpace(newStoredImagePath) && File.Exists(newStoredImagePath))
            {
                try
                {
                    File.Delete(newStoredImagePath);
                }
                catch (Exception cleanupException)
                {
                    _logger.LogWarning(
                        cleanupException,
                        "Unable to delete orphan image file after update-property failure: {FilePath}",
                        newStoredImagePath);
                }
            }

            _logger.LogError(ex, "Failed to update property {PropertyId}", id);
            return Result.Failure<Property>(
                code: "properties.update_failed",
                message: "Unable to update property.",
                statusCode: StatusCodes.Status500InternalServerError,
                detail: ex.Message);
        }
    }

    private async Task<Result<Property>?> ValidateImageFileAsync(IFormFile imageFile, CancellationToken cancellationToken)
    {
        if (imageFile.Length <= 0)
        {
            return Result.Failure<Property>(
                code: "properties.image_empty",
                message: "Image file is empty.",
                statusCode: StatusCodes.Status415UnsupportedMediaType);
        }

        if (imageFile.Length > MaxImageSizeBytes)
        {
            return Result.Failure<Property>(
                code: "properties.image_too_large",
                message: "Image file exceeds 5 MB.",
                statusCode: StatusCodes.Status413PayloadTooLarge);
        }

        var extension = Path.GetExtension(imageFile.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            return Result.Failure<Property>(
                code: "properties.image_not_supported",
                message: "Image file must be a PNG or JPG image.",
                statusCode: StatusCodes.Status415UnsupportedMediaType);
        }

        var contentType = imageFile.ContentType?.Trim().ToLowerInvariant();
        if (contentType is not ("image/png" or "image/jpeg"))
        {
            return Result.Failure<Property>(
                code: "properties.image_not_supported",
                message: "Image file must be a PNG or JPG image.",
                statusCode: StatusCodes.Status415UnsupportedMediaType);
        }

        await using var stream = imageFile.OpenReadStream();
        var header = new byte[8];
        var bytesRead = await stream.ReadAsync(header.AsMemory(0, header.Length), cancellationToken);

        if (!IsPng(header, bytesRead) && !IsJpeg(header, bytesRead))
        {
            return Result.Failure<Property>(
                code: "properties.image_not_supported",
                message: "Image file must be a valid PNG or JPG image.",
                statusCode: StatusCodes.Status415UnsupportedMediaType);
        }

        return null;
    }

    private string GetPropertiesDirectory()
    {
        var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        return Path.Combine(webRootPath, "assets", "properties");
    }

    private void TryDeleteStoredImage(string imageUrl)
    {
        try
        {
            var propertiesDirectory = GetPropertiesDirectory();
            var storedFileName = Path.GetFileName(imageUrl);
            if (string.IsNullOrWhiteSpace(storedFileName))
            {
                return;
            }

            var storedFilePath = Path.Combine(propertiesDirectory, storedFileName);
            if (File.Exists(storedFilePath))
            {
                File.Delete(storedFilePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unable to delete previous property image after update: {ImageUrl}", imageUrl);
        }
    }

    private static bool IsPng(byte[] header, int bytesRead)
    {
        return bytesRead >= 8
            && header[0] == 0x89
            && header[1] == 0x50
            && header[2] == 0x4E
            && header[3] == 0x47
            && header[4] == 0x0D
            && header[5] == 0x0A
            && header[6] == 0x1A
            && header[7] == 0x0A;
    }

    private static bool IsJpeg(byte[] header, int bytesRead)
    {
        return bytesRead >= 3
            && header[0] == 0xFF
            && header[1] == 0xD8
            && header[2] == 0xFF;
    }
}
