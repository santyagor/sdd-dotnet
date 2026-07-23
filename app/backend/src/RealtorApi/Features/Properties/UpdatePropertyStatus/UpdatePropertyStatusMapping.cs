using RealtorApi.Domain.Properties;

namespace RealtorApi.Features.Properties.UpdatePropertyStatus;

public static class UpdatePropertyStatusMapping
{
    public static UpdatePropertyStatusResponse ToResponse(this Property property)
    {
        ArgumentNullException.ThrowIfNull(property);

        return new UpdatePropertyStatusResponse(
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
}
