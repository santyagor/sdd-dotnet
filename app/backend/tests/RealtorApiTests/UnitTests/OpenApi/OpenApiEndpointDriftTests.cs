using System.Text.Json;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using FluentAssertions;

namespace RealtorApiTests.UnitTests.OpenApi;

public sealed class OpenApiEndpointDriftTests : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly IReadOnlySet<(string Method, string Path)> ExpectedEndpoints = new HashSet<(string Method, string Path)>
    {
        ("GET", "/health"),
        ("GET", "/api/properties"),
        ("GET", "/api/properties/{id}"),
        ("POST", "/api/properties"),
        ("PUT", "/api/properties/{id}")
    };

    private readonly WebApplicationFactory<Program> _factory;

    public OpenApiEndpointDriftTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void PublishedOpenApiDocument_StaysInSyncWithRuntimeEndpoints()
    {
        var runtimeEndpoints = CollectRuntimeEndpoints().ToHashSet();
        var documentEndpoints = CollectDocumentEndpoints().ToHashSet();

        runtimeEndpoints.Should().BeEquivalentTo(ExpectedEndpoints);
        documentEndpoints.Should().BeEquivalentTo(ExpectedEndpoints);
    }

    private IEnumerable<(string Method, string Path)> CollectRuntimeEndpoints()
    {
        var dataSource = _factory.Services.GetRequiredService<EndpointDataSource>();

        return dataSource.Endpoints
            .OfType<RouteEndpoint>()
            .SelectMany(endpoint =>
            {
                var methods = endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods ?? Array.Empty<string>();
                var normalizedRoute = EndpointRouteNormalization.Normalize(endpoint.RoutePattern.RawText ?? endpoint.RoutePattern.ToString() ?? string.Empty);

                return methods.Select(method => (method.ToUpperInvariant(), normalizedRoute));
            })
            .Where(endpoint => ExpectedEndpoints.Contains(endpoint));
    }

    private IEnumerable<(string Method, string Path)> CollectDocumentEndpoints()
    {
        var documentPath = GetOpenApiDocumentPath();
        using var document = JsonDocument.Parse(File.ReadAllText(documentPath));

        foreach (var pathProperty in document.RootElement.GetProperty("paths").EnumerateObject())
        {
            var normalizedPath = EndpointRouteNormalization.Normalize(pathProperty.Name);
            foreach (var operation in pathProperty.Value.EnumerateObject())
            {
                var method = operation.Name.ToUpperInvariant();
                var pair = (method, normalizedPath);
                if (ExpectedEndpoints.Contains(pair))
                {
                    yield return pair;
                }
            }
        }
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
