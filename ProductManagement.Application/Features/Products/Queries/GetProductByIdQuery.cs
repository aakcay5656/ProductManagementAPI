using MediatR;
using ProductManagement.Core.Common;
using ProductManagementAPI.ProductManagement.Application.DTOs.Products;

namespace ProductManagement.Application.Features.Products.Queries
{
    public class GetProductByIdQuery : IRequest<Result<ProductDto>>
    {
        public int Id { get; set; }
    }
}
