using Microsoft.AspNetCore.Http;
using RealtorApi.Features.Properties.ListProperties;

namespace RealtorApiTests.UnitTests.Features.Properties.ListProperties;

public class ListPropertiesMappingTests
{
    [Fact]
    public void BuildPublicImageUrl_UsesCurrentRequestHostAndAssetsRoute()
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("localhost", 5023);

        var url = ListPropertiesMapping.BuildPublicImageUrl("/images/properties/1.png", context);

        url.Should().Be("http://localhost:5023/assets/properties/1.png");
    }

    [Fact]
    public void ToResponse_TransformsProjectionIntoPublicImageUrl()
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");

        var projection = new PropertyListProjection(
            Guid.NewGuid(),
            "Casa A",
            "Descripción",
            "Dirección",
            1234m,
            PropertyStatus.Rented,
            2,
            1,
            88.5m,
            "7.png");

        var response = projection.ToResponse(context);

        response.ImageUrl.Should().Be("https://example.com/assets/properties/7.png");
        response.Title.Should().Be("Casa A");
        response.Status.Should().Be(PropertyStatus.Rented);
    }
}
