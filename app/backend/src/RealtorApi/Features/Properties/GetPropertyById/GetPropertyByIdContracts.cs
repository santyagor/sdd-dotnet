using RealtorApi.Domain.Properties;

namespace RealtorApi.Features.Properties.GetPropertyById;

public sealed record GetPropertyByIdRequest(string? Id);

public sealed record PropertyDetailResponse(
    Guid Id,
    string Title,
    string Description,
    string Address,
    decimal Price,
    PropertyStatus Status,
    int BedroomCount,
    int BathroomCount,
    decimal AreaSquareMeters,
    string ImageUrl);
