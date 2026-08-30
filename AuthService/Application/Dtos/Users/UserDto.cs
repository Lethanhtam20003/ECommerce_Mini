namespace AuthService.Application.Dtos.Users
{
    public record UserDto(
        Guid Id,
        string UserName,
        string Email,
        string Role);
}
