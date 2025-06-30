namespace Catalogue.Shared.Models
{
    public sealed class Category
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();

        public void AddProduct(Product product)
        {
            Products.Add(product);
        }
    }
}
