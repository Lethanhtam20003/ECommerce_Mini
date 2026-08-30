using AuthService.Application.Common.Models;
using AuthService.Application.Dtos.Users;
using AuthService.Domain.Common;
using MediatR;

namespace AuthService.Application.Features.Users.Queries
{
    public class GetUsersQuery : PaginationParams, IRequest<Result<PaginatedResult<UserDto>>>
    {
        public string? SearchTerm { get; init; }
    }
}
