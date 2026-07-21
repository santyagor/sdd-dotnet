using RealtorApi.Infrastructure.Api;
using RealtorApi.Infrastructure.Handlers;
using RealtorApi.Infrastructure.Validation;
using RealtorApi.Infrastructure.Results;
using RealtorApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting();
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

app.MapSlices();

app.MapGet("/", () => Results.Ok(new { status = "OK" }));

// Apply migrations and run seeding at startup (async)
await app.MigrateDatabaseAsync();
await app.UseAsyncSeeding();

app.Run();

public partial class Program { }
