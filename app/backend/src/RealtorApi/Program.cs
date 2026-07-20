using RealtorApi.Infrastructure.Api;
using RealtorApi.Infrastructure.Handlers;
using RealtorApi.Infrastructure.Validation;
using RealtorApi.Infrastructure.Results;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting();
builder.Services.AddSlices();
builder.Services.AddHandlers();
builder.Services.AddFluentValidation();
builder.Services.AddSingleton<ResultProblemDetailsMapper>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapSlices();

app.MapGet("/", () => Results.Ok(new { status = "OK" }));

app.Run();

public partial class Program { }
