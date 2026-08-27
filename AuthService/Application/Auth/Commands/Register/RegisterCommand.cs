using AuthService.Domain.Common;
using FluentValidation;
using MediatR;

namespace AuthService.Application.Auth.Commands.Register
{
    public record RegisterCommand(string Email, string Password) : IRequest<Result<RegisterResponse>>;
    public record RegisterResponse(string AccessToken, string Email, string Role);
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        }
    }
  

}

