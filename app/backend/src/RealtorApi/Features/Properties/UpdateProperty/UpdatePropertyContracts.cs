using Microsoft.AspNetCore.Http;
using RealtorApi.Domain.Properties;

namespace RealtorApi.Features.Properties.UpdateProperty;

public sealed class UpdatePropertyRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public decimal? Price { get; set; }
    public PropertyStatus? Status { get; set; }
    public int? BedroomCount { get; set; }
    public int? BathroomCount { get; set; }
    public decimal? AreaSquareMeters { get; set; }
    public IFormFile? ImageFile { get; set; }
}

public sealed record UpdatePropertyResponse(
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
