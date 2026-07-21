using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using RealtorApi.Domain.Properties;
using RealtorApi.Infrastructure.Persistence.Configurations;

namespace RealtorApiTests.UnitTests;

public class PropertyMappingTests
{
    [Fact]
    public void PropertyStatusIsConfiguredAsStringAndOtherPropertyConstraintsApply()
    {
        var modelBuilder = new ModelBuilder(new ConventionSet());
        new PropertyConfiguration().Configure(modelBuilder.Entity<Property>());

        var entityType = modelBuilder.Model.FindEntityType(typeof(Property));
        entityType.Should().NotBeNull();

        var statusProperty = entityType!.FindProperty(nameof(Property.Status));
        statusProperty.Should().NotBeNull();
        statusProperty!.GetValueConverter()?.ProviderClrType.Should().Be(typeof(string));
        statusProperty.GetMaxLength().Should().Be(50);
        statusProperty.IsNullable.Should().BeFalse();

        var priceProperty = entityType.FindProperty(nameof(Property.Price));
        priceProperty.Should().NotBeNull();
        priceProperty!.GetPrecision().Should().Be(18);
        priceProperty.GetScale().Should().Be(2);

        var imageUrlProperty = entityType.FindProperty(nameof(Property.ImageUrl));
        imageUrlProperty.Should().NotBeNull();
        imageUrlProperty!.GetMaxLength().Should().Be(2048);
    }
}
