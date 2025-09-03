namespace Products.Catalogue.Domain.Entities
{
    public class Brand : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
