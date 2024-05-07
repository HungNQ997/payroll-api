using System.Net;

namespace Payroll.SharedModel.Wrapper
{
    public class Result : IResult
    {
        public string Message { get; set; }
        public bool Succeeded { get; set; }
        public int Code { get; set; }

        public static IResult Fail(int statusCode = (int)HttpStatusCode.OK)
        {
            return new Result { Succeeded = false, Code = statusCode };
        }

        public static IResult Fail(string message, int statusCode = (int)HttpStatusCode.OK)
        {
            return new Result { Succeeded = false, Message = message, Code = statusCode };
        }

        public static Task<IResult> FailAsync(int statusCode = (int)HttpStatusCode.OK)
        {
            return Task.FromResult(Fail(statusCode));
        }

        public static Task<IResult> FailAsync(string message, int statusCode = (int)HttpStatusCode.OK)
        {
            return Task.FromResult(Fail(message, statusCode));
        }

        public static IResult Success(int statusCode = (int)HttpStatusCode.OK)
        {
            return new Result { Succeeded = true, Code = statusCode };
        }

        public static IResult Success(string message, int statusCode = (int)HttpStatusCode.OK)
        {
            return new Result { Succeeded = true, Message = message, Code = statusCode };
        }

        public static Task<IResult> SuccessAsync(int statusCode = (int)HttpStatusCode.OK)
        {
            return Task.FromResult(Success(statusCode));
        }

        public static Task<IResult> SuccessAsync(string message, int statusCode = (int)HttpStatusCode.OK)
        {
            return Task.FromResult(Success(message, statusCode));
        }
    }

    public class Result<T> : Result, IResult<T>
    {
        public T Data { get; set; }

        public new static Result<T> Fail(int statusCode = (int)HttpStatusCode.OK)
        {
            return new Result<T> { Succeeded = false, Code = statusCode };
        }

        public new static Result<T> Fail(string message, int statusCode = (int)HttpStatusCode.OK)
        {
            return new Result<T> { Succeeded = false, Message = message, Code = statusCode };
        }

        public static Result<T> Fail(string message, T data, int statusCode = (int)HttpStatusCode.OK)
        {
            return new Result<T> { Succeeded = false, Data = data, Message = message, Code = statusCode };
        }

        public new static Task<Result<T>> FailAsync(int statusCode = (int)HttpStatusCode.OK)
        {
            return Task.FromResult(Fail(statusCode));
        }

        public new static Task<Result<T>> FailAsync(string message, int statusCode = (int)HttpStatusCode.OK)
        {
            return Task.FromResult(Fail(message, statusCode));
        }

        public static Task<Result<T>> FailAsync(string Message, T data, int statusCode = (int)HttpStatusCode.OK)
        {
            return Task.FromResult(Fail(Message, data, statusCode));
        }

        public new static Result<T> Success(int statusCode = (int)HttpStatusCode.OK)
        {
            return new Result<T> { Succeeded = true, Code = statusCode };
        }

        public new static Result<T> Success(string message, int statusCode = (int)HttpStatusCode.OK)
        {
            return new Result<T> { Succeeded = true, Message = message, Code = statusCode };
        }

        public static Result<T> Success(T data, int statusCode = (int)HttpStatusCode.OK)
        {
            return new Result<T> { Succeeded = true, Data = data, Code = statusCode };
        }

        public static Result<T> Success(T data, string message, int statusCode = (int)HttpStatusCode.OK)
        {
            return new Result<T> { Succeeded = true, Data = data, Message = message, Code = statusCode };
        }

        public new static Task<Result<T>> SuccessAsync(int statusCode = (int)HttpStatusCode.OK)
        {
            return Task.FromResult(Success(statusCode));
        }

        public new static Task<Result<T>> SuccessAsync(string message, int statusCode = (int)HttpStatusCode.OK)
        {
            return Task.FromResult(Success(message, statusCode));
        }

        public static Task<Result<T>> SuccessAsync(T data, int statusCode = (int)HttpStatusCode.OK)
        {
            return Task.FromResult(Success(data, statusCode));
        }

        public static Task<Result<T>> SuccessAsync(T data, string message, int statusCode = (int)HttpStatusCode.OK)
        {
            return Task.FromResult(Success(data, message, statusCode));
        }
    }
}
