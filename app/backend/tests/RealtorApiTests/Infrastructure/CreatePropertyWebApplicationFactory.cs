using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApiTests.Infrastructure;

public sealed class CreatePropertyWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _webRootPath;
    private readonly string _databaseName;

    public CreatePropertyWebApplicationFactory(string webRootPath)
    {
        _webRootPath = webRootPath;
        _databaseName = Guid.NewGuid().ToString("N");
        Directory.CreateDirectory(_webRootPath);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseWebRoot(_webRootPath);

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}