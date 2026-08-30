using AuthService.Application.Common.Extensions;
using AuthService.Application.Common.Models;
using AuthService.Application.Dtos.Users;
using AuthService.Domain.Common;
using AuthService.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Application.Features.Users.Queries
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<PaginatedResult<UserDto>>>
    {
        private readonly AuthDbContext _context;

        public GetUsersQueryHandler(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResult<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Users.AsNoTracking().AsQueryable();

            return await query
            .Select(u => new UserDto(
                u.Id,
                u.UserName,
                u.Email,
                "USER"
            )).ToPaginatedListAsync(request.PageIndex, request.PageSize, cancellationToken);
        }
    }
}
