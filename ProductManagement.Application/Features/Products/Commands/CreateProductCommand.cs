using MediatR;
using ProductManagementAPI.ProductManagement.Application.DTOs.Products;
using ProductManagement.Core.Common;

namespace ProductManagement.Application.Features.Products.Commands
{
    public class CreateProductCommand : IRequest<Result<ProductDto>>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
