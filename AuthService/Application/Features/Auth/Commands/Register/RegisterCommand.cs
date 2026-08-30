using AuthService.Domain.Common;
using FluentValidation;
using MediatR;

namespace AuthService.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand(string UserName, string Email, string Password) : IRequest<Result<RegisterResponse>>;
    public record RegisterResponse(string AccessToken, string Username, string Role);
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {

            RuleFor(x => x.UserName).NotEmpty().MinimumLength(1).MaximumLength(200);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(200);
        }
    }
  

}

