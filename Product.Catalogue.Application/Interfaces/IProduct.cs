using Products.Catalogue.Domain.Entities;

namespace Products.Catalogue.Application.Interfaces
{
    public interface IProduct : IGenericInterface<Product>
    {
        Task<IEnumerable<Product>> GetAllWithDetailsAsync();
        Task<Product> GetByIdWithDetailsAsync(int id);
    }
}
