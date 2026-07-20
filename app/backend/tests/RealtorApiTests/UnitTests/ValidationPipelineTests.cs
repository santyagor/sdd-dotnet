using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace RealtorApiTests.UnitTests
{
    public class ValidationPipelineTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ValidationPipelineTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ValidationRequest_InvalidReturnsValidationProblemDetails()
        {
            var content = JsonContent.Create(new { Name = "", Age = 0 });
            var response = await _client.PostAsync("/validation", content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.Content.Headers.ContentType?.MediaType.Should().Contain("problem");

            var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            payload.RootElement.GetProperty("title").GetString().Should().Be("Validation failed");
            payload.RootElement.GetProperty("errors").GetProperty("Name").EnumerateArray().Should().ContainSingle();
        }

        [Fact]
        public async Task ValidationRequest_ValidContinuesSuccessfully()
        {
            var content = JsonContent.Create(new { Name = "Santi", Age = 25 });
            var response = await _client.PostAsync("/validation", content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            payload.RootElement.GetProperty("name").GetString().Should().Be("Santi");
            payload.RootElement.GetProperty("age").GetInt32().Should().Be(25);
        }

        [Fact]
        public async Task ValidationRequest_NoValidatorPassesThroughWithoutError()
        {
            var content = JsonContent.Create(new { Data = "anything" });
            var response = await _client.PostAsync("/validation-no-validator", content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            payload.RootElement.GetProperty("data").GetString().Should().Be("anything");
        }
    }
}
