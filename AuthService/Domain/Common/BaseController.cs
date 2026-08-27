using AuthService.Application.Common;
using AuthService.Domain.Common.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.Domain.Common
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BaseController : ControllerBase
    {
        private ISender? _sender;
        protected ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();
        // Helper method để chuyển đổi Result sang ActionResult
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return Ok(ApiResponse<T>.Success(result.Value!));

            return MapErrorToActionResult(result.Error);
        }
        private IActionResult MapErrorToActionResult(Error error)
        {
            var response = ApiResponse<object>.Failure(error.Code, error.Message);

            return error.Type switch
            {
                ErrorType.Validation => BadRequest(response),
                ErrorType.NotFound => NotFound(response),
                ErrorType.Unauthorized => Unauthorized(response),
                ErrorType.Forbidden => Forbid(), // Hoặc trả về 403 tùy chính sách
                ErrorType.Conflict => Conflict(response),
                _ => BadRequest(response) // Mặc định cho ErrorType.Failure
            };
        }
        protected Guid? AuthenticatedUserId
        {
            get
            {
                var claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(claimValue, out var id) ? id : null;
            }
        }
    }
}
