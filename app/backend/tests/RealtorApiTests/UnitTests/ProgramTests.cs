using System.Net.Http;

namespace RealtorApiTests.UnitTests;

public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProgramTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetRoot_ReturnsSuccessStatusCode()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/");

        response.IsSuccessStatusCode.Should().BeTrue();
    }
}
