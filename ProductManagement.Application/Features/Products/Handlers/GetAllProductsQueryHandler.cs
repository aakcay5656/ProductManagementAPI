using MediatR;
using ProductManagement.Application.Features.Products.Queries;
using ProductManagement.Core.Common;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;
using ProductManagementAPI.ProductManagement.Application.DTOs.Products;

namespace ProductManagement.Application.Features.Products.Handlers
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<ProductDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public GetAllProductsQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<Result<IEnumerable<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"products:all:{request.Category}:{request.SearchTerm}:{request.Page}:{request.PageSize}";

            // Try get from cache first
            var cachedProducts = await _cacheService.GetAsync<IEnumerable<ProductDto>>(cacheKey);
            if (cachedProducts != null)
            {
                return Result<IEnumerable<ProductDto>>.Success(cachedProducts);
            }

            // Get from database
            var products = await _unitOfWork.Products.GetAllAsync(p => p.User);

            // Filter active products only
            var filteredProducts = products.Where(p => p.IsActive && !p.IsDeleted);

            // Apply category filter
            if (!string.IsNullOrEmpty(request.Category))
            {
                filteredProducts = filteredProducts.Where(p =>
                    p.Category.ToLower().Contains(request.Category.ToLower()));
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                filteredProducts = filteredProducts.Where(p =>
                    p.Name.ToLower().Contains(request.SearchTerm.ToLower()) ||
                    p.Description.ToLower().Contains(request.SearchTerm.ToLower()));
            }

            // Apply pagination
            var pagedProducts = filteredProducts
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(MapToDto)
                .ToList();

            // Cache for 5 minutes
            await _cacheService.SetAsync(cacheKey, pagedProducts, TimeSpan.FromMinutes(5));

            return Result<IEnumerable<ProductDto>>.Success(pagedProducts);
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
