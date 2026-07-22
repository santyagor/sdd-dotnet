using RealtorApi.Domain.Properties;

namespace RealtorApi.Features.Properties.CreateProperty;

public static class CreatePropertyMapping
{
    public static CreatePropertyResponse ToResponse(this Property property)
    {
        return new CreatePropertyResponse(
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