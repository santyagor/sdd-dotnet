using Microsoft.AspNetCore.Http;
using FluentValidation;

namespace RealtorApi.Features.Properties.CreateProperty;

public sealed class CreatePropertyValidator : AbstractValidator<CreatePropertyRequest>
{
    private const long MaxImageSizeBytes = 5L * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png",
        ".jpg",
        ".jpeg"
    };

    public CreatePropertyValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .WithMessage("Title is required.");

        RuleFor(request => request.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(request => request.Address)
            .NotEmpty()
            .WithMessage("Address is required.");

        RuleFor(request => request.Price)
            .NotNull()
            .WithMessage("Price is required.")
            .GreaterThan(0m)
            .WithMessage("Price must be greater than zero.");

        RuleFor(request => request.Status)
            .NotNull()
            .WithMessage("Status is required.")
            .IsInEnum()
            .WithMessage("Status must be a valid property status.");

        RuleFor(request => request.BedroomCount)
            .NotNull()
            .WithMessage("BedroomCount is required.")
            .GreaterThanOrEqualTo(0)
            .WithMessage("BedroomCount must be zero or greater.");

        RuleFor(request => request.BathroomCount)
            .NotNull()
            .WithMessage("BathroomCount is required.")
            .GreaterThanOrEqualTo(0)
            .WithMessage("BathroomCount must be zero or greater.");

        RuleFor(request => request.AreaSquareMeters)
            .NotNull()
            .WithMessage("AreaSquareMeters is required.")
            .GreaterThan(0m)
            .WithMessage("AreaSquareMeters must be greater than zero.");

        RuleFor(request => request.ImageFile)
            .MustAsync(BeValidImageFileAsync)
            .WithMessage("ImageFile must be a PNG or JPG image no larger than 5 MB.");
    }

    private static async Task<bool> BeValidImageFileAsync(IFormFile? imageFile, CancellationToken cancellationToken)
    {
        if (imageFile is null)
        {
            return true;
        }

        if (imageFile.Length is <= 0 or > MaxImageSizeBytes)
        {
            return false;
        }

        var extension = Path.GetExtension(imageFile.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            return false;
        }

        var contentType = imageFile.ContentType?.Trim().ToLowerInvariant();
        if (contentType is not ("image/png" or "image/jpeg"))
        {
            return false;
        }

        await using var stream = imageFile.OpenReadStream();
        var header = new byte[8];
        var read = await stream.ReadAsync(header.AsMemory(0, header.Length), cancellationToken);
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        return IsPng(header, read) || IsJpeg(header, read);
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