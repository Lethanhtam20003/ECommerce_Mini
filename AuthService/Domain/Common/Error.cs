using AuthService.Domain.Common.Enums;

namespace AuthService.Domain.Common
{
    public record Error(string Code, string Message, ErrorType Type = ErrorType.Failure)
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
        public static readonly Error NullValue = new("Error.NullValue", "Giá trị cung cấp là null.", ErrorType.Validation);
        public static Error Create(string code, string message, ErrorType type = ErrorType.Failure) => new(code, message, type);
        public static Error Unauthorized() => Error.Create("Unauthorized", "You are not authorized.", ErrorType.BadRequest);
        public static Error MapperError() => Error.Create("Mapper.ExecutionFailed", "An error occurred while attempting to map the objects.", ErrorType.BadRequest);

    }
   
}
