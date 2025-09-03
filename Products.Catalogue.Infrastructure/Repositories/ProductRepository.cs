using Microsoft.EntityFrameworkCore;
using Products.Catalogue.Application.Interfaces;
using Products.Catalogue.Domain.Entities;
using Products.Catalogue.Infrastructure.Data;
using SharedLibrary.Logs;
using SharedLibrary.Resonse;
using System.Linq.Expressions;

namespace Products.Catalogue.Infrastructure.Repositories
{
    public class ProductRepository : IProduct
    {
        private readonly ProductDbContext _db;

        public ProductRepository(ProductDbContext db)
        {
            _db = db;
        }
        public async Task<ApiRespose> AddAsync(Product entity)
        {
            try
            {
                var getProduct = await GetByAsync(_ => _.Name!.Equals(entity.Name));
                if (getProduct is not null && !string.IsNullOrEmpty(entity.Name))
                {
                    return new ApiRespose(false, $"Product {entity.Name} already exists....");
                }


                var currentProduct = await _db.Products.AddAsync(entity);
                await _db.SaveChangesAsync();

                if (entity.Id > 0 && currentProduct is not null)
                {
                    return new ApiRespose(true, $"Product {entity.Name} added successfully", entity);
                }
                else
                {
                    return new ApiRespose(false, $"Failed to add {entity.Name} product....");
                }
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);

                return new ApiRespose(false, "Error occured while adding Product....");
            }
        }

        public async Task<ApiRespose> FindIdAsync(int id)
        {
            try
            {
                var product = await _db.Products.FindAsync(id);
                if (product is not null)
                {
                    return new ApiRespose(true, $"Product with id {id} retrived successfully", product);
                }
                else
                {
                    return new ApiRespose(false, $"Product with id {id} does not exists....");
                }
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);

                return new ApiRespose(false, $"Error occured while finding/retriving id {id} Product....");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var product = await _db.Products.AsNoTracking().ToListAsync();
                return product is not null ? product : null;
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);

                throw new InvalidOperationException("Error occured while retriving Product....");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await _db.Products.Where(predicate).FirstOrDefaultAsync()!;
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                throw new InvalidOperationException("Error occured while retriving Product....");
            }
        }

        public async Task<ApiRespose> UpdateAsync(Product entity)
        {
            try
            {
                var existingProduct = await _db.Products.FindAsync(entity.Id);

                if (existingProduct is null)
                {
                    return new ApiRespose(false, $"Product with id {entity.Id} does not exist....");
                }

                _db.Entry(existingProduct).State = EntityState.Detached;
                _db.Products.Update(entity);
                await _db.SaveChangesAsync();

                return new ApiRespose(true, $"Product with id {entity.Id} updated successfully....", entity);
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                return new ApiRespose(false, "Error occurred while updating Product....");
            }
        }

        public async Task<ApiRespose> DeleteAsync(Product entity)
        {
            try
            {
                var response = await FindIdAsync(entity.Id);

                if (!response.flags || response.Data is not Product product)
                {
                    return new ApiRespose(false, $"Product with id {entity.Id} does not exist....");
                }

                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
                return new ApiRespose(true, $"Product with id {entity.Id} deleted successfully....");
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                return new ApiRespose(false, "Error occurred while deleting Product....");
            }
        }
        // This is the special method for getting a single product with all its details
        public async Task<Product> GetByIdWithDetailsAsync(int id)
        {
            return await _db.Products
                .Include(p => p.Category) // Eagerly loads the Category
                .Include(p => p.Brand)    // Eagerly loads the Brand
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // This is the special method for getting ALL products with their details
        public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
        {
            return await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}