namespace ProductManagement.Core.Interfaces
{
    public interface IAuthService
    {
        Task<string> GenerateJwtTokenAsync(int userId, string email, string role);
        Task<bool> ValidateTokenAsync(string token);
        string HashPassword(string password);
        bool VerifyPassword(string password,string hashedPassword);
    }
}
