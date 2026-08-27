using AuthService.Domain.Common;
using AuthService.Domain.Common.Enums;
using FluentValidation;
using MediatR;
using System.Reflection;

namespace AuthService.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // 1. Bỏ qua nếu Command/Query này không có Validator nào được định nghĩa
        if (!_validators.Any())
        {
            return await next();
        }

        // 2. Chạy toàn bộ các rules của Validator bất đồng bộ
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        // 3. Nếu có lỗi validation, ngắt pipeline và trả về Result.Failure
        if (failures.Count != 0)
        {
            var firstError = failures[0];
            var error = new Error(
                Code: firstError.PropertyName,
                Message: firstError.ErrorMessage,
                Type: ErrorType.Validation
            );

            return CreateFailureResult(error);
        }

        // 4. Dữ liệu hợp lệ -> Đi tiếp vào Handler
        return await next();
    }

    private static TResponse CreateFailureResult(Error error)
    {
        var responseType = typeof(TResponse);

        // Kiểm tra xem TResponse có phải là kiểu Result<TValue> không
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var genericArgument = responseType.GetGenericArguments()[0];

            // Tìm method static Failure(Error error) của Result<TValue>
            var failureMethod = typeof(Result<>)
                .MakeGenericType(genericArgument)
                .GetMethod(nameof(Result<object>.Failure), BindingFlags.Public | BindingFlags.Static, new[] { typeof(Error) });

            if (failureMethod is not null)
            {
                return (TResponse)failureMethod.Invoke(null, new object[] { error })!;
            }
        }

        throw new InvalidOperationException($"Kiểu trả về {responseType.Name} không kế thừa từ Result<TValue>.");
    }
}