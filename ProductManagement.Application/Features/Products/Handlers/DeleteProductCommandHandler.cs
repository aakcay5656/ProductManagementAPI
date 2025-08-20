using MediatR;
using ProductManagement.Application.Features.Products.Commands;
using ProductManagement.Core.Common;
using ProductManagement.Core.Interfaces;

namespace ProductManagement.Application.Features.Products.Handlers
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.Id);

            if (product == null)
            {
                return Result.Failure("Product not found");
            }

            // Check if user owns the product
            if (product.UserId != request.UserId)
            {
                return Result.Failure("You can only delete your own products");
            }

            // Soft delete
            product.IsDeleted = true;
            product.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            // Cache invalidation
            await _cacheService.RemoveByPatternAsync("products:*");
            await _cacheService.RemoveAsync($"product:{request.Id}");

            return Result.Success();
        }
    }
}
