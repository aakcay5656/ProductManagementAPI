using ProductManagement.Core.Entities;

namespace ProductManagement.Core.Interfaces
{
    public interface IUserRepository: IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<IEnumerable<User>> GetActiveUsersAsync();
    }
}
