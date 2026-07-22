using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using RealtorApi.Features.Properties.CreateProperty;
using RealtorApi.Infrastructure.Api;
using RealtorApiTests.Infrastructure;

namespace RealtorApiTests.UnitTests.Features.Properties.CreateProperty;

public class CreatePropertySliceTests
{
    [Fact]
    public void Slice_IsDiscoverableAsSlice()
    {
        typeof(CreatePropertySlice).Should().Implement<ISlice>();
    }

    [Fact]
    public void Register_MapsCreatePropertyEndpoint()
    {
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        var slice = new CreatePropertySlice();

        slice.Register(app);

        var endpoints = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(dataSource => dataSource.Endpoints)
            .OfType<RouteEndpoint>()
            .ToList();

        var route = endpoints.Single(endpoint => endpoint.RoutePattern.RawText == "/api/properties");
        route.DisplayName.Should().Contain("HTTP: POST");
    }

    [Fact]
    public async Task Post_CreateProperty_ReturnsCreatedResponseAndPersistsProperty()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var webRoot = Path.Combine(tempRoot, "wwwroot");

        try
        {
            using var factory = new CreatePropertyWebApplicationFactory(webRoot);
            using var client = factory.CreateClient();

            var title = $"Casa endpoint {Guid.NewGuid():N}";
            var form = BuildFormData(title);

            var response = await client.PostAsync("/api/properties", form);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

            var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            payload.RootElement.GetProperty("title").GetString().Should().Be(title);
            payload.RootElement.GetProperty("imageUrl").ValueKind.Should().Be(JsonValueKind.Null);

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var persisted = await db.Properties.SingleAsync(property => property.Title == title);
            persisted.ImageUrl.Should().BeNull();

            var expectedFile = Directory.EnumerateFiles(Path.Combine(webRoot, "assets", "properties")).SingleOrDefault();
            expectedFile.Should().BeNull();
        }
        finally
        {
            Directory.Delete(tempRoot, true);
        }
    }

    private static MultipartFormDataContent BuildFormData(string title)
    {
        var form = new MultipartFormDataContent();
        form.Add(new StringContent(title), nameof(CreatePropertyRequest.Title));
        form.Add(new StringContent("Casa creada desde endpoint"), nameof(CreatePropertyRequest.Description));
        form.Add(new StringContent("456 Endpoint Ave"), nameof(CreatePropertyRequest.Address));
        form.Add(new StringContent("3500"), nameof(CreatePropertyRequest.Price));
        form.Add(new StringContent(PropertyStatus.Available.ToString()), nameof(CreatePropertyRequest.Status));
        form.Add(new StringContent("4"), nameof(CreatePropertyRequest.BedroomCount));
        form.Add(new StringContent("3"), nameof(CreatePropertyRequest.BathroomCount));
        form.Add(new StringContent("165.5"), nameof(CreatePropertyRequest.AreaSquareMeters));
        return form;
    }
}