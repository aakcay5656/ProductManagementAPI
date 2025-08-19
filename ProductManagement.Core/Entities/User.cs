

using ProductManagement.Core.Enums;

namespace ProductManagement.Core.Entities
{
    public class User:BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;


        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    }
}
