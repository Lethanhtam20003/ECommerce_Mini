namespace AuthService.Application.Common
{
    public record ApiError(string Code, string Message, string? Details = null);

    public record ApiResponse<T>
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
        public ApiError? Error { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        // Static Factory Methods giúp code sạch hơn (Clean Code)
        public static ApiResponse<T> Success(T data) => new()
        {
            IsSuccess = true,
            Data = data
        };

        public static ApiResponse<T> Failure(string code, string message, string? details = null) => new()
        {
            IsSuccess = false,
            Error = new ApiError(code, message, details)
        };
    }
}
