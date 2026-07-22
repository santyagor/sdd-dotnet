using RealtorApi.Features.Properties.CreateProperty;

namespace RealtorApiTests.UnitTests.Features.Properties.CreateProperty;

public class CreatePropertyMappingTests
{
    [Fact]
    public void ToResponse_CopiesPersistedImageUrlAndPropertyData()
    {
        var property = new Property
        {
            Id = Guid.NewGuid(),
            Title = "Casa mapeada",
            Description = "Descripción",
            Address = "Dirección",
            Price = 1800m,
            Status = PropertyStatus.Maintenance,
            BedroomCount = 2,
            BathroomCount = 1,
            AreaSquareMeters = 95m,
            ImageUrl = "/assets/properties/abc123.png",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var response = property.ToResponse();

        response.Id.Should().Be(property.Id);
        response.Title.Should().Be(property.Title);
        response.ImageUrl.Should().Be("/assets/properties/abc123.png");
    }

    [Fact]
    public void ToResponse_AllowsNullImageUrl()
    {
        var property = new Property
        {
            Id = Guid.NewGuid(),
            Title = "Casa sin imagen",
            Description = "Descripción",
            Address = "Dirección",
            Price = 1800m,
            Status = PropertyStatus.Available,
            BedroomCount = 2,
            BathroomCount = 1,
            AreaSquareMeters = 95m,
            ImageUrl = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var response = property.ToResponse();

        response.ImageUrl.Should().BeNull();
    }
}