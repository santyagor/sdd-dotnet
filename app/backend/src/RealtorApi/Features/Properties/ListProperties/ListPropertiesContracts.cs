using RealtorApi.Domain.Properties;

namespace RealtorApi.Features.Properties.ListProperties;

public sealed record ListPropertiesQuery(int Page = 1, int PageSize = 6);

public sealed record PropertyListItem(
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

public sealed record PaginatedPropertyListResponse(
    IReadOnlyCollection<PropertyListItem> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasNext,
    bool HasPrevious);

public sealed record PropertyListProjection(
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
