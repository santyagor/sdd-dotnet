using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealtorApi.Infrastructure.Handlers;
using RealtorApi.Infrastructure.Persistence;
using RealtorApi.Infrastructure.Results;

namespace RealtorApi.Features.Properties.UpdatePropertyStatus;

public sealed class UpdatePropertyStatusHandler : IHandler
{
    private readonly AppDbContext _dbContext;

    public UpdatePropertyStatusHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UpdatePropertyStatusResponse>> HandleAsync(
        Guid id,
        UpdatePropertyStatusRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var property = await _dbContext.Properties
            .FirstOrDefaultAsync(currentProperty => currentProperty.Id == id, cancellationToken);

        if (property is null)
        {
            return Result.Failure<UpdatePropertyStatusResponse>(
                code: "properties.not_found",
                message: "Property not found.",
                statusCode: StatusCodes.Status404NotFound,
                detail: $"Property '{id}' was not found.");
        }

        property.Status = request.Status!.Value;
        property.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(property.ToResponse());
    }
}
