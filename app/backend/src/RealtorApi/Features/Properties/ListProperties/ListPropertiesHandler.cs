using Microsoft.EntityFrameworkCore;
using RealtorApi.Infrastructure.Handlers;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApi.Features.Properties.ListProperties;

public sealed class ListPropertiesHandler : IHandler
{
    private readonly AppDbContext _dbContext;

    public ListPropertiesHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedPropertyListResponse> HandleAsync(
        ListPropertiesQuery request,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var baseQuery = _dbContext.Properties
            .AsNoTracking()
            .OrderBy(property => property.Title);

        var totalItems = await baseQuery.CountAsync(cancellationToken);
        var totalPages = totalItems == 0
            ? 0
            : (int)Math.Ceiling(totalItems / (double)request.PageSize);

        var pageIndex = Math.Max(request.Page - 1, 0);
        var projections = await baseQuery
            .Skip(pageIndex * request.PageSize)
            .Take(request.PageSize)
            .Select(property => property.ToProjection())
            .ToListAsync(cancellationToken);

        var items = projections
            .Select(projection => projection.ToResponse(httpContext))
            .ToArray();

        return new PaginatedPropertyListResponse(
            items,
            request.Page,
            request.PageSize,
            totalItems,
            totalPages,
            request.Page < totalPages,
            request.Page > 1);
    }
}
