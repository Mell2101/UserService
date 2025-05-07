using System;

namespace UserService.Services;

public readonly struct Result
{
    public bool IsSuccess { get; }
    public string Error { get; }
    
    private Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Ok() => new(true, null);
    public static Result Fail(string error) => new(false, error);
    
    public static Result<T> Ok<T>(T value) => new(value, true, null);
    public static Result<T> Fail<T>(string error) => new(default, false, error);
}

public readonly struct Result<T>
{
    public T Value { get; }
    public bool IsSuccess { get; }
    public string Error { get; }
    
    internal Result(T value, bool isSuccess, string error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }
}
