using MediatR;
using ProductManagement.Core.Common;
using ProductManagementAPI.ProductManagement.Application.DTOs.Products;

namespace ProductManagement.Application.Features.Products.Queries
{
    public class GetAllProductsQuery : IRequest<Result<IEnumerable<ProductDto>>>
    {
        public string? Category { get; set; }
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
