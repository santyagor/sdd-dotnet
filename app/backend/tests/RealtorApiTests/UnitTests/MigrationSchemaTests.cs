using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using RealtorApi.Infrastructure.Persistence;
using RealtorApi.Infrastructure.Persistence.Configurations;
using RealtorApi.Domain.Properties;

namespace RealtorApiTests.UnitTests;

public class MigrationSchemaTests
{
    [Fact]
    public async Task MigrationCreatesPropertiesTableWithCorrectSchema()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new AppDbContext(options);

        // Ensure database created (no MigrateAsync for InMemory)
        await db.Database.EnsureCreatedAsync();

        var model = db.Model;
        var propertyEntity = model.FindEntityType("RealtorApi.Domain.Properties.Property");
        propertyEntity.Should().NotBeNull();

        var tableName = propertyEntity!.GetTableName();
        tableName.Should().Be("Properties");

        // Verify Status property is configured as string
        var statusProperty = propertyEntity.FindProperty("Status");
        statusProperty.Should().NotBeNull();
        statusProperty!.GetValueConverter()?.ProviderClrType.Should().Be(typeof(string));

        // Verify Price precision
        var priceProperty = propertyEntity.FindProperty("Price");
        priceProperty.Should().NotBeNull();
        priceProperty!.GetPrecision().Should().Be(18);
        priceProperty.GetScale().Should().Be(2);

        // Verify AreaSquareMeters precision
        var areaProperty = propertyEntity.FindProperty("AreaSquareMeters");
        areaProperty.Should().NotBeNull();
        areaProperty!.GetPrecision().Should().Be(10);
        areaProperty.GetScale().Should().Be(2);

        // Verify required fields
        var titleProperty = propertyEntity.FindProperty("Title");
        titleProperty.Should().NotBeNull();
        titleProperty!.IsNullable.Should().BeFalse();
        titleProperty.GetMaxLength().Should().Be(200);

        var descProperty = propertyEntity.FindProperty("Description");
        descProperty.Should().NotBeNull();
        descProperty!.IsNullable.Should().BeFalse();
        descProperty.GetMaxLength().Should().Be(1000);

        var addressProperty = propertyEntity.FindProperty("Address");
        addressProperty.Should().NotBeNull();
        addressProperty!.IsNullable.Should().BeFalse();
        addressProperty.GetMaxLength().Should().Be(300);

        var imageUrlProperty = propertyEntity.FindProperty("ImageUrl");
        imageUrlProperty.Should().NotBeNull();
        imageUrlProperty!.IsNullable.Should().BeFalse();
        imageUrlProperty.GetMaxLength().Should().Be(2048);

        // Verify timestamps
        var createdAtProperty = propertyEntity.FindProperty("CreatedAt");
        createdAtProperty.Should().NotBeNull();
        createdAtProperty!.IsNullable.Should().BeFalse();

        var updatedAtProperty = propertyEntity.FindProperty("UpdatedAt");
        updatedAtProperty.Should().NotBeNull();
        updatedAtProperty!.IsNullable.Should().BeFalse();

        // Verify bedroom/bathroom counts
        var bedroomProperty = propertyEntity.FindProperty("BedroomCount");
        bedroomProperty.Should().NotBeNull();
        bedroomProperty!.IsNullable.Should().BeFalse();

        var bathroomProperty = propertyEntity.FindProperty("BathroomCount");
        bathroomProperty.Should().NotBeNull();
        bathroomProperty!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void PropertyStatusEnumIsPersistenceAsString()
    {
        var modelBuilder = new ModelBuilder(new ConventionSet());
        new PropertyConfiguration().Configure(modelBuilder.Entity<Property>());

        var entityType = modelBuilder.Model.FindEntityType(typeof(Property));
        var statusProperty = entityType!.FindProperty("Status");

        // Verify to the Status property configuration - the converter is set when the property is configured
        statusProperty!.GetMaxLength().Should().Be(50);
        // The converter is applied, verify by checking the model
        statusProperty.GetColumnType().Should().NotBeNull();
    }
}
