using System.Text.Json.Serialization;
using RealtorApi.Domain.Properties;

namespace RealtorApi.Features.Properties.UpdatePropertyStatus;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class UpdatePropertyStatusRequest
{
    public PropertyStatus? Status { get; set; }
}

public sealed record UpdatePropertyStatusResponse(
    Guid Id,
    string Title,
    string Description,
    string Address,
    decimal Price,
    PropertyStatus Status,
    int BedroomCount,
    int BathroomCount,
    decimal AreaSquareMeters,
    string? ImageUrl);
