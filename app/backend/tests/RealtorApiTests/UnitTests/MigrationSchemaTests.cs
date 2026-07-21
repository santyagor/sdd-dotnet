using System.IO;

namespace RealtorApiTests.UnitTests;

public class MigrationSchemaTests
{
    [Fact]
    public void MigrationDefinesStatusColumnAsString()
    {
        var migrationPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "..",
            "src",
            "RealtorApi",
            "Migrations",
            "20260721020842_InitialPropertySchema.cs"));

        File.Exists(migrationPath).Should().BeTrue();

        var migration = File.ReadAllText(migrationPath);

        migration.Should().Contain("Status = table.Column<string>");
        migration.Should().Contain("maxLength: 50");
        migration.Should().Contain("nullable: false");
    }

    [Fact]
    public void PropertyConfigurationUsesStringConversionForStatus()
    {
        var configurationPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "..",
            "src",
            "RealtorApi",
            "Infrastructure",
            "Persistence",
            "Configurations",
            "PropertyConfiguration.cs"));

        File.Exists(configurationPath).Should().BeTrue();

        var configuration = File.ReadAllText(configurationPath);

        configuration.Should().Contain("HasConversion<string>()");
        configuration.Should().Contain("HasMaxLength(50)");
    }
}
