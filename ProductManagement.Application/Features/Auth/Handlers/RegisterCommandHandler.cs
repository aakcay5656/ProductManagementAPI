using MediatR;

using ProductManagement.Core.Common;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Enums;
using ProductManagement.Core.Interfaces;
using ProductManagementAPI.ProductManagement.Application.DTOs.Auth;
using ProductManagementAPI.ProductManagement.Application.Features.Auth.Commands;

namespace ProductManagement.Application.Features.Auth.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;

        public RegisterCommandHandler(IUnitOfWork unitOfWork, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Check if user already exists
            if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
            {
                return Result<AuthResponseDto>.Failure("User with this email already exists");
            }

            // Create new user
            var user = new User
            {
                Email = request.Email,
                Password = _authService.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = UserRole.User,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdUser = await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Generate JWT token
            var token = await _authService.GenerateJwtTokenAsync(
                createdUser.Id,
                createdUser.Email,
                createdUser.Role.ToString()
            );

            var response = new AuthResponseDto
            {
                Token = token,
                Email = createdUser.Email,
                FullName = $"{createdUser.FirstName} {createdUser.LastName}",
                Role = createdUser.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            return Result<AuthResponseDto>.Success(response);
        }
    }
}
