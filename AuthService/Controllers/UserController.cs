using AuthService.Application.Auth.Commands.Register;
using AuthService.Application.Dtos.Auth;
using AuthService.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class Usercontroller : BaseController
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
