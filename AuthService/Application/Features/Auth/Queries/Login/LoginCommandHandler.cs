using AuthService.Application.Features.Auth.Commands.Register;
using AuthService.Application.Interface;
using AuthService.Domain.Common;
using AuthService.Domain.Common.Enums;
using AuthService.Domain.Entities;
using MediatR;

namespace AuthService.Application.Features.Auth.Queries.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<RegisterCommandHandler> _logger;
        public LoginCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService, ILogger<RegisterCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _logger = logger;

        }

        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("attempting login for {Email}", request.Email);
            // kiểm tra tài khoản tồn tại 
            var user = await _userRepository.GetUserAsync(request.Email, tracking: false, cancellationToken); 
            if (user is null)
            {
                return Result<LoginResponse>.Failure(new Error("User.Conflict", "Email không tồn tại.", ErrorType.Conflict));
            }
            // kiểm tra mật khẩu
            if(!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Result<LoginResponse>.Failure(new Error("User.Password", "Mật khẩu không chính xác.", ErrorType.Conflict));
            }

            // gentoken
            var token = _jwtTokenService.GenerateToken(user);

            return Result<LoginResponse>.Success(new LoginResponse(token, "name",request.Email, "user"));
        }
    }
}
