using System.Net;
using System.Text.Json;

namespace RealtorApiTests.UnitTests
{
    public class RegisterSlicesTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public RegisterSlicesTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RegisterSlices_DiscoverTestSliceWithoutDuplicates()
        {
            var response = await _client.GetAsync("/test-health");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

            var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            payload.RootElement.GetProperty("status").GetString().Should().Be("Healthy");
            payload.RootElement.GetProperty("source").GetString().Should().Be("test-slice");
        }
    }
}
