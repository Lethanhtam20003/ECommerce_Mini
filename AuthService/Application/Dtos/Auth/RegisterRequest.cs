namespace AuthService.Application.Dtos.Auth
{
    public record RegisterRequest(String UserName, string Email, string Password);
}
