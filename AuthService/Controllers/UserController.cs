using AuthService.Application.Features.Users.Queries;
using AuthService.Controllers.Common;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : BaseController
    {

        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] GetUsersQuery query,
            CancellationToken cancellationToken)
        {
            var result = await Sender.Send(query, cancellationToken);
            return HandleResult(result);

        }
    }
}
