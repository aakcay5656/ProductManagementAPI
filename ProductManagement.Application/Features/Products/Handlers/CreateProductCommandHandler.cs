using MediatR;
using ProductManagement.Application.Features.Products.Commands;
using ProductManagement.Core.Common;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;
using ProductManagementAPI.ProductManagement.Application.DTOs.Products;

namespace ProductManagement.Application.Features.Products.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Verify user exists
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<ProductDto>.Failure("User not found");
            }

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                Category = request.Category,
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdProduct = await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            // Cache invalidation
            await _cacheService.RemoveByPatternAsync("products:*");

            // Load user for response
            var productWithUser = await _unitOfWork.Products.GetByIdAsync(createdProduct.Id, p => p.User);

            var productDto = MapToDto(productWithUser!);
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
