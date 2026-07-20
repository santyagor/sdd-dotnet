using Microsoft.AspNetCore.Http;

namespace RealtorApi.Infrastructure.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public ResultError? Error { get; }

        protected Result(bool isSuccess, ResultError? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new Result(true, null);

        public static Result Failure(string code, string message, int statusCode = StatusCodes.Status400BadRequest, string? detail = null)
            => new Result(false, new ResultError(code, message, statusCode, detail));

        public static Result<T> Success<T>(T value) => new Result<T>(value);

        public static Result<T> Failure<T>(string code, string message, int statusCode = StatusCodes.Status400BadRequest, string? detail = null)
            => new Result<T>(default!, new ResultError(code, message, statusCode, detail), false);
    }

    public sealed class Result<T> : Result
    {
        public T Value { get; }

        internal Result(T value)
            : base(true, null)
        {
            Value = value;
        }

        internal Result(T value, ResultError error, bool isSuccess)
            : base(isSuccess, error)
        {
            Value = value;
        }
    }

    public sealed class ResultError
    {
        public string Code { get; }
        public string Message { get; }
        public int StatusCode { get; }
        public string? Detail { get; }

        public ResultError(string code, string message, int statusCode, string? detail)
        {
            Code = code;
            Message = message;
            StatusCode = statusCode;
            Detail = detail;
        }
    }
}
