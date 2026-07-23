using FluentAssertions;

namespace RealtorApiTests.UnitTests.OpenApi;

public sealed class OpenApiNswagSmokeTests
{
    [Fact]
    public async Task GeneratedClientWithNswag_CompletesWithoutManualEdits()
    {
        var repoRoot = GetRepositoryRoot();
        var openApiDocumentPath = Path.Combine(repoRoot, "app", "backend", "src", "RealtorApi", "wwwroot", "openapi", "v1.json");
        var outputDirectory = Path.Combine(repoRoot, "artifacts", "openapi-client-smoke");
        var outputFile = Path.Combine(outputDirectory, "RealtorApiClient.cs");

        File.Exists(openApiDocumentPath).Should().BeTrue();
        Directory.CreateDirectory(outputDirectory);
        if (File.Exists(outputFile))
        {
            File.Delete(outputFile);
        }

        await ExternalToolProcessRunner.RunAsync("dotnet", "tool restore", repoRoot);

        var toolListResult = await ExternalToolProcessRunner.RunAsync("dotnet", "tool list", repoRoot);
        toolListResult.StandardOutput.Should().Contain("nswag.consolecore");

        await ExternalToolProcessRunner.RunAsync(
            "dotnet",
            $"tool run nswag openapi2csclient /input:\"{openApiDocumentPath}\" /output:\"{outputFile}\" /classname:RealtorApiClient /namespace:RealtorApi.OpenApiSmoke",
            repoRoot);

        File.Exists(outputFile).Should().BeTrue();
        var clientContent = await File.ReadAllTextAsync(outputFile);
        clientContent.Should().Contain("class RealtorApiClient");
    }

    private static string GetRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var specPath = Path.Combine(directory.FullName, "specs", "008-open-api", "spec.md");
            if (File.Exists(specPath))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Unable to locate repository root.");
    }
}
