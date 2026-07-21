using Microsoft.AspNetCore.Http;
using RealtorApi.Features.Properties.ListProperties;

namespace RealtorApiTests.UnitTests.Features.Properties.ListProperties;

public class ListPropertiesHandlerTests
{
    [Fact]
    public async Task HandleAsync_OrdersByTitleAndAppliesPaginationDefaults()
    {
        var dbName = Guid.NewGuid().ToString("N");
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        await using var db = new AppDbContext(options);
        db.Properties.AddRange(
            CreateProperty("b-title", "B title", "2.png"),
            CreateProperty("a-title", "A title", "1.png"),
            CreateProperty("c-title", "C title", "3.png"));
        await db.SaveChangesAsync();

        var handler = new ListPropertiesHandler(db);
        var context = new DefaultHttpContext();
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("localhost", 5023);

        var response = await handler.HandleAsync(new ListPropertiesQuery(), context, CancellationToken.None);

        response.Page.Should().Be(1);
        response.PageSize.Should().Be(6);
        response.TotalItems.Should().Be(3);
        response.TotalPages.Should().Be(1);
        response.HasNext.Should().BeFalse();
        response.HasPrevious.Should().BeFalse();
        response.Items.Should().HaveCount(3);
        response.Items.Select(item => item.Title).Should().ContainInOrder("A title", "B title", "C title");
        response.Items.Should().OnlyContain(item => item.ImageUrl.StartsWith("http://localhost:5023/assets/properties/"));
    }

    [Fact]
    public async Task HandleAsync_RespectsRequestedPage()
    {
        var dbName = Guid.NewGuid().ToString("N");
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        await using var db = new AppDbContext(options);
        db.Properties.AddRange(
            CreateProperty("alpha", "Alpha", "1.png"),
            CreateProperty("beta", "Beta", "2.png"),
            CreateProperty("delta", "Delta", "4.png"),
            CreateProperty("gamma", "Gamma", "3.png"),
            CreateProperty("theta", "Theta", "5.png"),
            CreateProperty("zeta", "Zeta", "6.png"),
            CreateProperty("omega", "Omega", "7.png"));
        await db.SaveChangesAsync();

        var handler = new ListPropertiesHandler(db);
        var context = new DefaultHttpContext();
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("localhost", 5023);

        var response = await handler.HandleAsync(new ListPropertiesQuery(2, 6), context, CancellationToken.None);

        response.Page.Should().Be(2);
        response.PageSize.Should().Be(6);
        response.TotalItems.Should().Be(7);
        response.TotalPages.Should().Be(2);
        response.HasNext.Should().BeFalse();
        response.HasPrevious.Should().BeTrue();
        response.Items.Should().HaveCount(1);
        response.Items.Single().Title.Should().Be("Zeta");
    }

    private static Property CreateProperty(string suffix, string title, string imageFileName)
    {
        return new Property
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = $"Description {suffix}",
            Address = $"Address {suffix}",
            Price = 1000m,
            Status = PropertyStatus.Available,
            BedroomCount = 2,
            BathroomCount = 1,
            AreaSquareMeters = 100m,
            ImageUrl = imageFileName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
