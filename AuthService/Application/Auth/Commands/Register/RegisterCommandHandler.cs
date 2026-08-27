using AuthService.Application.Interface;
using AuthService.Domain.Common;
using AuthService.Domain.Common.Enums;
using AuthService.Domain.Entities;
using MediatR;

namespace AuthService.Application.Auth.Commands.Register
{
    public class RegisterCommandHandler: IRequestHandler<RegisterCommand, Result<RegisterResponse>>
    {

        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<RegisterCommandHandler> _logger;
        public RegisterCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService, ILogger<RegisterCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
              
        }
        public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // kiểm tra email tồn tại
            if( await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                return Result<RegisterResponse>.Failure(new Error("User.Conflict", "Email đã tồn tại.", ErrorType.Conflict));
            }
            // hash password
            var hashedPassword = _passwordHasher.HashPassword(request.Password);
           
            // thêm user vào database
            User user = User.Create(request.Email, hashedPassword);
            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            // gentoken
            var token = _jwtTokenService.GenerateToken(user);

            return Result<RegisterResponse>.Success(new RegisterResponse(token,request.Email, "user"));
        }
    }
}
