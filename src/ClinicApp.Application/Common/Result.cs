namespace ClinicApp.Application.Common;

public class Result<T>
{
    public T? Value { get; }
    public Error? Error { get; }
    public bool IsSuccess => Error is null;

    private Result(T value)  => Value = value;
    private Result(Error error) => Error = error;

    public static Result<T> Ok(T value) => new(value);
    public static Result<T> Fail(Error error) => new(error);

    // implicit conversion — servis kodu temiz kalır
    public static implicit operator Result<T>(T value) => Ok(value);
    public static implicit operator Result<T>(Error error) => Fail(error);
}

// Değer döndürmeyen işlemler için (Delete gibi)
public class Result
{
    public Error? Error { get; }
    public bool IsSuccess => Error is null;

    private Result() { }
    private Result(Error error) => Error = error;

    public static Result Ok() => new();
    public static Result Fail(Error error) => new(error);

    public static implicit operator Result(Error error) => Fail(error);
}
