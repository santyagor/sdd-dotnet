using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RealtorApi.Infrastructure.Results
{
    public class ResultProblemDetailsMapper
    {
        private readonly ILogger<ResultProblemDetailsMapper> _logger;

        public ResultProblemDetailsMapper(ILogger<ResultProblemDetailsMapper> logger)
        {
            _logger = logger;
        }

        public IResult MapResult(Result result)
        {
            if (result.IsSuccess)
            {
                return Microsoft.AspNetCore.Http.Results.Ok();
            }

            _logger.LogWarning("Expected result failure mapped to ProblemDetails: {Error}", result.Error);
            return Microsoft.AspNetCore.Http.Results.Problem(
                detail: result.Error?.Detail,
                statusCode: result.Error?.StatusCode,
                title: result.Error?.Message,
                type: result.Error?.Code);
        }

        public IResult MapResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Microsoft.AspNetCore.Http.Results.Ok(result.Value);
            }

            _logger.LogWarning("Expected result failure mapped to ProblemDetails: {Error}", result.Error);
            return Microsoft.AspNetCore.Http.Results.Problem(
                detail: result.Error?.Detail,
                statusCode: result.Error?.StatusCode,
                title: result.Error?.Message,
                type: result.Error?.Code);
        }
    }
}
