namespace RealtorApiTests.UnitTests.OpenApi;

public sealed class EndpointRouteNormalizationTests
{
    [Theory]
    [InlineData("/api/properties/{id:guid}", "/api/properties/{id}")]
    [InlineData("/api/properties/{id:int}", "/api/properties/{id}")]
    [InlineData("/api/properties/{id:min(1)}", "/api/properties/{id}")]
    [InlineData("/health", "/health")]
    public void Normalize_RemovesRouteConstraints(string input, string expected)
    {
        EndpointRouteNormalization.Normalize(input).Should().Be(expected);
    }
}
