namespace AuthService.Domain.Common;

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    // Ngăn chặn truy cập Value khi Result là thất bại
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Không thể truy cập Value khi thao tác thất bại.");

    protected Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public static Result<TValue> Success(TValue value) => new(value, true, Error.None);
    public new static Result<TValue> Failure(Error error) => new(default, false, error);

    // Cho phép return thẳng dữ liệu hoặc return thẳng Error
    public static implicit operator Result<TValue>(TValue value) => Success(value);
    public static implicit operator Result<TValue>(Error error) => Failure(error);
}