using RealtorApi.Infrastructure.Api;
using RealtorApi.Infrastructure.Handlers;
using RealtorApi.Infrastructure.Validation;
using RealtorApi.Infrastructure.Results;
using RealtorApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting();
builder.Services.AddOpenApi("v1");
builder.Services.AddSlices();
builder.Services.AddHandlers();
builder.Services.AddFluentValidation();
builder.Services.AddSingleton<ResultProblemDetailsMapper>();

// EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<DatabaseSeeder>();

var app = builder.Build();

app.UseHttpsRedirection();

// Serve static files from wwwroot (default) and ensure images can be served
app.UseStaticFiles();
var propertiesImagesPath = Path.Combine(app.Environment.WebRootPath ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot"), "assets", "properties");
Directory.CreateDirectory(propertiesImagesPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(propertiesImagesPath),
    RequestPath = "/assets/properties"
});

app.MapSlices();

app.MapGet("/", () => Results.Ok(new { status = "OK" }));

// Apply migrations and run seeding at startup (async)
try
{
    await app.MigrateDatabaseAsync();
    await app.UseAsyncSeeding();
}
catch (Exception ex)
{
    app.Logger.LogWarning(ex, "Database initialization failed on startup; continuing without migration/seed.");
}

app.Run();

public partial class Program { }
