namespace ProductManagementAPI.ProductManagement.Application.DTOs.Products
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock {  get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt {  get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
    }

    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; }

        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;

    }

    public class UpdateProductDto {
        public string Name { get; set; } = string.Empty;
        public string Description {get; set;} = string.Empty;
        public decimal Price { get; set;}
        public int Stock { get; set;}
        public string Category { get; set;} = string.Empty; 
        public bool IsActive { get; set;}
    }
}
