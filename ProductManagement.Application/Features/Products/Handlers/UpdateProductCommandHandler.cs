using MediatR;
using ProductManagement.Application.Features.Products.Commands;
using ProductManagement.Core.Common;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;
using ProductManagementAPI.ProductManagement.Application.DTOs.Products;

namespace ProductManagement.Application.Features.Products.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<Result<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.Id, p => p.User);

            if (product == null)
            {
                return Result<ProductDto>.Failure("Product not found");
            }

            // Check if user owns the product (simple authorization)
            if (product.UserId != request.UserId)
            {
                return Result<ProductDto>.Failure("You can only update your own products");
            }

            // Update product properties
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.Category = request.Category;
            product.IsActive = request.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            var updatedProduct = await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            // Cache invalidation
            await _cacheService.RemoveByPatternAsync("products:*");
            await _cacheService.RemoveAsync($"product:{request.Id}");

            var productDto = MapToDto(updatedProduct);
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
