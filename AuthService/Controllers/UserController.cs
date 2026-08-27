using AuthService.Application.Dtos.Auth;
using AuthService.Application.Features.Auth.Commands.Register;
using AuthService.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(request.Email, request.Password);
            var result = await Sender.Send(command, cancellationToken);
            return HandleResult(result);
        }

    }
}
