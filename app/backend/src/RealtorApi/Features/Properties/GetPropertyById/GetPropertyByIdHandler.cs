using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealtorApi.Infrastructure.Handlers;
using RealtorApi.Infrastructure.Persistence;
using RealtorApi.Infrastructure.Results;

namespace RealtorApi.Features.Properties.GetPropertyById;

public sealed class GetPropertyByIdHandler : IHandler
{
    private readonly AppDbContext _dbContext;

    public GetPropertyByIdHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PropertyDetailResponse>> HandleAsync(
        GetPropertyByIdRequest request,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(httpContext);

        if (!Guid.TryParse(request.Id, out var id))
        {
            return Result.Failure<PropertyDetailResponse>(
                code: "properties.invalid_id",
                message: "Invalid property id.",
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Property id '{request.Id}' is not a valid GUID.");
        }

        var property = await _dbContext.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(currentProperty => currentProperty.Id == id, cancellationToken);

        if (property is null)
        {
            return Result.Failure<PropertyDetailResponse>(
                code: "properties.not_found",
                message: "Property not found.",
                statusCode: StatusCodes.Status404NotFound,
                detail: $"Property '{id}' was not found.");
        }

        return Result.Success(property.ToResponse(httpContext));
    }
}
