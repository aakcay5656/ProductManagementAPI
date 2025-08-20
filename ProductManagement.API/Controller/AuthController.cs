using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductManagementAPI.ProductManagement.Application.DTOs.Auth;
using ProductManagementAPI.ProductManagement.Application.Features.Auth.Commands;

namespace ProductManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var command = new RegisterCommand
            {
                Email = registerDto.Email,
                Password = registerDto.Password,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });
            }

            return Ok(new { message = "User registered successfully", data = result.Data });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var command = new LoginCommand
            {
                Email = loginDto.Email,
                Password = loginDto.Password
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return Unauthorized(new { message = result.ErrorMessage });
            }

            return Ok(new { message = "Login successful", data = result.Data });
        }
    }
}
