using MediatR;
using ProductManagement.Core.Common;
using ProductManagement.Core.Interfaces;
using ProductManagementAPI.ProductManagement.Application.DTOs.Auth;
using ProductManagementAPI.ProductManagement.Application.Features.Auth.Commands;

namespace ProductManagement.Application.Features.Auth.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;

        public LoginCommandHandler(IUnitOfWork unitOfWork, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

            if (user == null || !user.IsActive)
            {
                return Result<AuthResponseDto>.Failure("Invalid email or password");
            }

            if (!_authService.VerifyPassword(request.Password, user.Password))
            {
                return Result<AuthResponseDto>.Failure("Invalid email or password");
            }

            var token = await _authService.GenerateJwtTokenAsync(
                user.Id,
                user.Email,
                user.Role.ToString()
            );

            var response = new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            return Result<AuthResponseDto>.Success(response);
        }
    }
}
