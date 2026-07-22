using RealtorApi.Domain.Properties;

namespace RealtorApi.Features.Properties.UpdateProperty;

public static class UpdatePropertyMapping
{
    public static UpdatePropertyResponse ToResponse(this Property property)
    {
        return new UpdatePropertyResponse(
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

    public static string BuildPublicImageUrl(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        return $"/assets/properties/{Uri.EscapeDataString(fileName)}";
    }
}
