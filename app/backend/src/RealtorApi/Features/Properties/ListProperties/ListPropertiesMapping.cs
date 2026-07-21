using System.IO;
using RealtorApi.Domain.Properties;

namespace RealtorApi.Features.Properties.ListProperties;

public static class ListPropertiesMapping
{
    public static PropertyListProjection ToProjection(this Property property)
    {
        return new PropertyListProjection(
            property.Id,
            property.Title,
            property.Description,
            property.Address,
            property.Price,
            property.Status,
            property.BedroomCount,
            property.BathroomCount,
            property.AreaSquareMeters,
            property.ImageUrl);
    }

    public static PropertyListItem ToResponse(this PropertyListProjection projection, HttpContext httpContext)
    {
        return new PropertyListItem(
            projection.Id,
            projection.Title,
            projection.Description,
            projection.Address,
            projection.Price,
            projection.Status,
            projection.BedroomCount,
            projection.BathroomCount,
            projection.AreaSquareMeters,
            BuildPublicImageUrl(projection.ImageUrl, httpContext));
    }

    public static string BuildPublicImageUrl(string? persistedImageUrl, HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var fileName = Path.GetFileName(persistedImageUrl ?? string.Empty);
        var path = string.IsNullOrWhiteSpace(fileName)
            ? "/assets/properties/"
            : $"/assets/properties/{Uri.EscapeDataString(fileName)}";

        return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{path}";
    }
}
