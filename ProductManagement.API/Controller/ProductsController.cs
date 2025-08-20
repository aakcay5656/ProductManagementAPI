using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagementAPI.ProductManagement.Application.DTOs.Products;
using ProductManagement.Application.Features.Products.Commands;
using ProductManagement.Application.Features.Products.Queries;
using System.Security.Claims;

namespace ProductManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts([FromQuery] string? category, [FromQuery] string? search,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAllProductsQuery
            {
                Category = category,
                SearchTerm = search,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });
            }

            return Ok(new { data = result.Data, page, pageSize });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct(int id)
        {
            var query = new GetProductByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                return NotFound(new { message = result.ErrorMessage });
            }

            return Ok(new { data = result.Data });
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            var userId = GetCurrentUserId();

            var command = new CreateProductCommand
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                Stock = createProductDto.Stock,
                Category = createProductDto.Category,
                UserId = userId
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });
            }

            return CreatedAtAction(nameof(GetProduct), new { id = result.Data!.Id },
                new { message = "Product created successfully", data = result.Data });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            var userId = GetCurrentUserId();

            var command = new UpdateProductCommand
            {
                Id = id,
                Name = updateProductDto.Name,
                Description = updateProductDto.Description,
                Price = updateProductDto.Price,
                Stock = updateProductDto.Stock,
                Category = updateProductDto.Category,
                IsActive = updateProductDto.IsActive,
                UserId = userId
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });
            }

            return Ok(new { message = "Product updated successfully", data = result.Data });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var userId = GetCurrentUserId();

            var command = new DeleteProductCommand
            {
                Id = id,
                UserId = userId
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { message = "Product deleted successfully" });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}
