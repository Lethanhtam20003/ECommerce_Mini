using AuthService.Domain.Common;
using FluentValidation;
using MediatR;

namespace AuthService.Application.Features.Auth.Queries.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;

    public record LoginResponse(string Token, string UserName, string Email, string Role);

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator() 
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        }
    }
}
