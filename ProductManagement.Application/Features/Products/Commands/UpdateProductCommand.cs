using MediatR;
using ProductManagementAPI.ProductManagement.Application.DTOs.Products;
using ProductManagement.Core.Common;

namespace ProductManagement.Application.Features.Products.Commands
{
    public class UpdateProductCommand : IRequest<Result<ProductDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int UserId { get; set; }
    }
}
