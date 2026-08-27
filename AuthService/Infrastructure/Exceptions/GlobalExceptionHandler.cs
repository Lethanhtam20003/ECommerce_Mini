using AuthService.Application.Dtos.common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static AuthService.Infrastructure.Exceptions.ExceptionCustoms;

namespace AuthService.Infrastructure.Exceptions;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment env) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. Logging lỗi hệ thống
        logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        // 2. Xử lý trường hợp Client ngắt kết nối (CancellationToken được kích hoạt)
        if (exception is OperationCanceledException or TaskCanceledException)
        {
            logger.LogInformation("Yêu cầu đã bị hủy bởi người dùng hoặc hệ thống quá tải.");
            httpContext.Response.StatusCode = 499; // Client Closed Request
            return true;
        }

        // 3. Phân loại mã lỗi & Thông điệp an toàn
        var (statusCode, errorCode, message) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "NOT_FOUND", exception.Message),
            BadRequestException => (StatusCodes.Status400BadRequest, "BAD_REQUEST", exception.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "UNAUTHORIZED", exception.Message),
            ForbiddenException => (StatusCodes.Status403Forbidden, "FORBIDDEN", exception.Message),
            ConflictException => (StatusCodes.Status409Conflict, "CONFLICT", exception.Message),

            // Đối với lỗi 500: Ẩn chi tiết nội bộ nếu ở Production
            _ => (
                StatusCodes.Status500InternalServerError,
                "INTERNAL_SERVER_ERROR",
                env.IsDevelopment() ? exception.Message : "Đã xảy ra lỗi hệ thống nội bộ. Vui lòng thử lại sau."
            )
        };

        // 4. Chuẩn bị Response theo định dạng ApiResponse thống nhất
        var response = ApiResponse<object>.Failure(errorCode, message);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}