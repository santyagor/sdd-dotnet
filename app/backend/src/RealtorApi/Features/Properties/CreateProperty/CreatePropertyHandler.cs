using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using RealtorApi.Domain.Properties;
using RealtorApi.Infrastructure.Handlers;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApi.Features.Properties.CreateProperty;

public sealed class CreatePropertyHandler : IHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<CreatePropertyHandler> _logger;

    public CreatePropertyHandler(
        AppDbContext dbContext,
        IWebHostEnvironment environment,
        ILogger<CreatePropertyHandler> logger)
    {
        _dbContext = dbContext;
        _environment = environment;
        _logger = logger;
    }

    public async Task<Property> HandleAsync(CreatePropertyRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var timestamp = DateTime.UtcNow;
        var property = new Property
        {
            Id = Guid.NewGuid(),
            Title = request.Title!.Trim(),
            Description = request.Description!.Trim(),
            Address = request.Address!.Trim(),
            Price = request.Price!.Value,
            Status = request.Status!.Value,
            BedroomCount = request.BedroomCount!.Value,
            BathroomCount = request.BathroomCount!.Value,
            AreaSquareMeters = request.AreaSquareMeters!.Value,
            ImageUrl = null,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };

        string? storedFilePath = null;

        try
        {
            if (request.ImageFile is not null)
            {
                var propertiesDirectory = GetPropertiesDirectory();
                Directory.CreateDirectory(propertiesDirectory);

                var extension = Path.GetExtension(request.ImageFile.FileName);
                var storedFileName = $"{Guid.NewGuid():N}{extension}";
                storedFilePath = Path.Combine(propertiesDirectory, storedFileName);
                var publicImageUrl = CreatePropertyMapping.BuildPublicImageUrl(storedFileName);

                await using (var fileStream = File.Create(storedFilePath))
                {
                    await request.ImageFile.CopyToAsync(fileStream, cancellationToken);
                }

                property.ImageUrl = publicImageUrl;
            }

            _dbContext.Properties.Add(property);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return property;
        }
        catch (Exception ex)
        {
            if (storedFilePath is not null && File.Exists(storedFilePath))
            {
                try
                {
                    File.Delete(storedFilePath);
                }
                catch (Exception cleanupException)
                {
                    _logger.LogWarning(
                        cleanupException,
                        "Unable to delete orphan image file after create-property failure: {FilePath}",
                        storedFilePath);
                }
            }

            _logger.LogError(ex, "Failed to create property {PropertyTitle}", request.Title);
            throw;
        }
    }

    private string GetPropertiesDirectory()
    {
        var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        return Path.Combine(webRootPath, "assets", "properties");
    }
}