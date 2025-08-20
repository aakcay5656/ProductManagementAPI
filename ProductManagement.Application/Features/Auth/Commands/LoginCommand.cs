using MediatR;
using ProductManagementAPI.ProductManagement.Application.DTOs.Auth;
using ProductManagement.Core.Common;

namespace ProductManagementAPI.ProductManagement.Application.Features.Auth.Commands
{
    public class LoginCommand : IRequest<Result<AuthResponseDto>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
