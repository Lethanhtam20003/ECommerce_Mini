namespace AuthService.Domain.Common.Enums
{
    public enum ErrorType
    {
        None = 0,
        Failure = 1,        // Lỗi logic chung (400)
        Validation = 2,     // Lỗi dữ liệu đầu vào (400)
        Unauthorized = 3,   // Lỗi xác thực (401)
        Forbidden = 4,      // Lỗi phân quyền (403)
        NotFound = 5,       // Không tìm thấy tài nguyên (404)
        Conflict = 6,        // Xung đột dữ liệu (409)
        BadRequest = 7,
    }
}
