using Microsoft.EntityFrameworkCore;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;
using ProductManagement.Infrastructure.Data;

namespace ProductManagement.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(p => p.User) 
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _dbSet
                .Include(p => p.User) 
                .Where(p => p.IsActive && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            return await _dbSet
                .Include(p => p.User)
                .Where(p => p.Category.ToLower().Contains(category.ToLower())
                           && p.IsActive
                           && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
        {
            var term = searchTerm.ToLower();

            return await _dbSet
                .Include(p => p.User)
                .Where(p => (p.Name.ToLower().Contains(term)
                           || p.Description.ToLower().Contains(term))
                           && p.IsActive
                           && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}
