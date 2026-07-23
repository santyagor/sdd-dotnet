using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace RealtorApiTests.UnitTests.OpenApi;

public sealed class OpenApiSharedSchemaTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OpenApiSharedSchemaTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void PublishedDocument_ExposesSharedSchemas()
    {
        var documentPath = GetOpenApiDocumentPath();
        using var document = JsonDocument.Parse(File.ReadAllText(documentPath));

        var schemas = document.RootElement
            .GetProperty("components")
            .GetProperty("schemas")
            .EnumerateObject()
            .Select(property => property.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        schemas.Should().Contain("PropertyStatus");
        schemas.Should().Contain("ProblemDetails");
        schemas.Should().Contain("HttpValidationProblemDetails");
    }

    private string GetOpenApiDocumentPath()
    {
        var webRootPath = _factory.Services.GetRequiredService<IWebHostEnvironment>().WebRootPath
            ?? throw new InvalidOperationException("WebRootPath is not configured.");

        var path = Path.Combine(webRootPath, "openapi", "v1.json");
        File.Exists(path).Should().BeTrue($"OpenAPI document must exist at {path}");
        return path;
    }
}
