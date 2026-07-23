using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;

namespace RealtorApiTests.UnitTests.OpenApi;

public sealed class OpenApiRedoclyLintTests
{
    [Fact]
    public async Task LintingThePublishedDocument_WithRedocly_CompletesSuccessfully()
    {
        var repoRoot = GetRepositoryRoot();
        var openApiDocumentPath = Path.Combine(repoRoot, "app", "backend", "src", "RealtorApi", "wwwroot", "openapi", "v1.json");
        var redoclyConfigPath = Path.Combine(repoRoot, ".redocly.yaml");

        File.Exists(openApiDocumentPath).Should().BeTrue();
        File.Exists(redoclyConfigPath).Should().BeTrue();

        await ExternalToolProcessRunner.RunAsync(
            "npx",
            $"--yes @redocly/cli lint --config \"{redoclyConfigPath}\" \"{openApiDocumentPath}\"",
            repoRoot);
    }

    private static string GetRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var redoclyConfigPath = Path.Combine(directory.FullName, ".redocly.yaml");
            if (File.Exists(redoclyConfigPath))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Unable to locate repository root.");
    }
}
