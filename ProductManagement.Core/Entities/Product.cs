namespace ProductManagement.Core.Entities
{
    public class Product:BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price {get; set;}
        public int Stock { get; set;}
        public string Category { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        // Foreign Key
        public int UserId {  get; set; }

        // Navigation Property
        public virtual User User { get; set; } = null!;

    }
}
