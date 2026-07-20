var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok(new { status = "Healthy" }));

app.Run();

public partial class Program { }
