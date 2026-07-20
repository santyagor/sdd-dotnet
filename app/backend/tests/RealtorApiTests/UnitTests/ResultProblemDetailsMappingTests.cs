using System.Net;
using System.Text.Json;

namespace RealtorApiTests.UnitTests
{
    public class ResultProblemDetailsMappingTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ResultProblemDetailsMappingTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ErrorResult_ReturnsProblemDetailsWithExpectedStatus()
        {
            var response = await _client.GetAsync("/error-result");

            response.StatusCode.Should().Be((HttpStatusCode)422);
            response.Content.Headers.ContentType?.MediaType.Should().Contain("problem");

            var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            payload.RootElement.GetProperty("title").GetString().Should().Be("Expected error occurred");
            payload.RootElement.GetProperty("type").GetString().Should().Be("expected_error");
            payload.RootElement.GetProperty("status").GetInt32().Should().Be(422);
        }
    }
}
