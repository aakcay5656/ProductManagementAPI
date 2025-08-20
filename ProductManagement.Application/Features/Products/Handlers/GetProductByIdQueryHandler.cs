using MediatR;
using ProductManagement.Application.Features.Products.Queries;
using ProductManagement.Core.Common;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;
using ProductManagementAPI.ProductManagement.Application.DTOs.Products;

namespace ProductManagement.Application.Features.Products.Handlers
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"product:{request.Id}";

            // Try get from cache first
            var cachedProduct = await _cacheService.GetAsync<ProductDto>(cacheKey);
            if (cachedProduct != null)
            {
                return Result<ProductDto>.Success(cachedProduct);
            }

            // Get from database
            var product = await _unitOfWork.Products.GetByIdAsync(request.Id, p => p.User);

            if (product == null || product.IsDeleted)
            {
                return Result<ProductDto>.Failure("Product not found");
            }

            var productDto = MapToDto(product);

            // Cache for 10 minutes
            await _cacheService.SetAsync(cacheKey, productDto, TimeSpan.FromMinutes(10));

            return Result<ProductDto>.Success(productDto);
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Category = product.Category,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                UserName = $"{product.User?.FirstName} {product.User?.LastName}".Trim()
            };
        }
    }
}
