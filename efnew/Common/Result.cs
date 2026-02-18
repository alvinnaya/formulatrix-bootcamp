namespace Common;

public class Result
{
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }
    public int StatusCode { get; init; }

    public static Result Ok(int statusCode = 200)
        => new() { IsSuccess = true, StatusCode = statusCode };

    public static Result Fail(string error, int statusCode = 400)
        => new() { IsSuccess = false, Error = error, StatusCode = statusCode };
}

public class Result<T> : Result
{
    public T? Data { get; init; }

    public static Result<T> Ok(T data, int statusCode = 200)
        => new() { IsSuccess = true, Data = data, StatusCode = statusCode };

    public static new Result<T> Fail(string error, int statusCode = 400)
        => new() { IsSuccess = false, Error = error, StatusCode = statusCode };
}
