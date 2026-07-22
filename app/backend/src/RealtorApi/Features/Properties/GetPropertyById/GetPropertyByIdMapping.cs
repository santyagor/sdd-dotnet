using System.IO;
using Microsoft.AspNetCore.Http;
using RealtorApi.Domain.Properties;

namespace RealtorApi.Features.Properties.GetPropertyById;

public static class GetPropertyByIdMapping
{
    public static PropertyDetailResponse ToResponse(this Property property, HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(httpContext);

        return new PropertyDetailResponse(
            property.Id,
            property.Title,
            property.Description,
            property.Address,
            property.Price,
            property.Status,
            property.BedroomCount,
            property.BathroomCount,
            property.AreaSquareMeters,
            BuildPublicImageUrl(property.ImageUrl, httpContext));
    }

    public static string BuildPublicImageUrl(string? persistedImageUrl, HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        if (string.IsNullOrWhiteSpace(persistedImageUrl))
        {
            return string.Empty;
        }

        var fileName = Path.GetFileName(persistedImageUrl);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return string.Empty;
        }

        var path = $"/assets/properties/{Uri.EscapeDataString(fileName)}";
        return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{path}";
    }
}
