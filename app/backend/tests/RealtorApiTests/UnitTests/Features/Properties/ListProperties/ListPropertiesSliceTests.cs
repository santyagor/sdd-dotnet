using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RealtorApi.Features.Properties.ListProperties;
using RealtorApi.Infrastructure.Api;

namespace RealtorApiTests.UnitTests.Features.Properties.ListProperties;

public class ListPropertiesSliceTests
{
    [Fact]
    public void Slice_IsDiscoverableAsSlice()
    {
        typeof(ListPropertiesSlice).Should().Implement<ISlice>();
    }

    [Fact]
    public void Register_MapsPropertiesEndpoint()
    {
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        var slice = new ListPropertiesSlice();

        slice.Register(app);

        var dataSources = ((IEndpointRouteBuilder)app).DataSources.ToList();
        dataSources.Should().NotBeEmpty();

        var endpoints = dataSources
            .SelectMany(dataSource => dataSource.Endpoints)
            .OfType<RouteEndpoint>()
            .ToList();

        var route = endpoints.Single(endpoint => endpoint.RoutePattern.RawText == "/api/properties");
        route.DisplayName.Should().Contain("HTTP: GET");
    }
}
