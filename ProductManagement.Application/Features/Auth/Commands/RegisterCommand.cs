using MediatR;
using ProductManagementAPI.ProductManagement.Application.DTOs.Auth;
using ProductManagement.Core.Common;

namespace ProductManagementAPI.ProductManagement.Application.Features.Auth.Commands
{
    public class RegisterCommand : IRequest<Result<AuthResponseDto>>
    {
        public string Email {  get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;    
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;


    }
}
