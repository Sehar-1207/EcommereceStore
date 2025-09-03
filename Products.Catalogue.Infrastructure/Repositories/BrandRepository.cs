using Microsoft.EntityFrameworkCore;
using Products.Catalogue.Application.Interfaces;
using Products.Catalogue.Domain.Entities;
using Products.Catalogue.Infrastructure.Data;
using SharedLibrary.Logs;
using SharedLibrary.Resonse;
using System.Linq.Expressions;

namespace Products.Catalogue.Infrastructure.Repositories
{
    public class BrandRepository : IBrand 
    {
        private readonly ProductDbContext _db;

        public BrandRepository(ProductDbContext db)
        {
            _db = db;
        }

        public async Task<ApiRespose> AddAsync(Brand entity)
        {
            try
            {
                var getBrand = await GetByAsync(b => b.Name!.Equals(entity.Name));
                if (getBrand is not null && !string.IsNullOrEmpty(entity.Name))
                {
                    return new ApiRespose(false, $"Brand '{entity.Name}' already exists.");
                }

                await _db.Brands.AddAsync(entity);
                await _db.SaveChangesAsync();

                if (entity.Id > 0)
                {
                    return new ApiRespose(true, $"Brand '{entity.Name}' added successfully.", entity);
                }
                else
                {
                    return new ApiRespose(false, $"Failed to add '{entity.Name}' brand.");
                }
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                return new ApiRespose(false, "An error occurred while adding the brand.");
            }
        }

        public async Task<ApiRespose> FindIdAsync(int id)
        {
            try
            {
                var brand = await _db.Brands.FindAsync(id);
                if (brand is not null)
                {
                    return new ApiRespose(true, $"Brand with ID {id} retrieved successfully.", brand);
                }
                else
                {
                    return new ApiRespose(false, $"Brand with ID {id} does not exist.");
                }
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                return new ApiRespose(false, $"An error occurred while finding brand with ID {id}.");
            }
        }

        public async Task<IEnumerable<Brand>> GetAllAsync()
        {
            try
            {
                return await _db.Brands.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                throw new InvalidOperationException("An error occurred while retrieving brands.");
            }
        }

        public async Task<Brand> GetByAsync(Expression<Func<Brand, bool>> predicate)
        {
            try
            {
                return await _db.Brands.Where(predicate).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                throw new InvalidOperationException("An error occurred while retrieving a brand.");
            }
        }

        public async Task<ApiRespose> UpdateAsync(Brand entity)
        {
            try
            {
                var existingBrand = await _db.Brands.FindAsync(entity.Id);
                if (existingBrand is null)
                {
                    return new ApiRespose(false, $"Brand with ID {entity.Id} does not exist.");
                }

                _db.Entry(existingBrand).CurrentValues.SetValues(entity);
                await _db.SaveChangesAsync();

                return new ApiRespose(true, $"Brand with ID {entity.Id} updated successfully.", entity);
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                return new ApiRespose(false, "An error occurred while updating the brand.");
            }
        }

        public async Task<ApiRespose> DeleteAsync(Brand entity)
        {
            try
            {
                // Use the ID from the provided entity to find the actual record in the database This ensures we are working with an entity tracked by Entity Framework.
                var brandToDelete = await _db.Brands.FindAsync(entity.Id);

                // Check if a brand with that ID was found.
                if (brandToDelete is null)
                {
                    return new ApiRespose(false, $"Brand with ID {entity.Id} does not exist.");
                }

                // If found, remove it from the DbContext.
                _db.Brands.Remove(brandToDelete);

                // Commit the change to the database.
                await _db.SaveChangesAsync();

                // Return a successful response.
                return new ApiRespose(true, $"Brand with ID {entity.Id} deleted successfully.");
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                return new ApiRespose(false, "An error occurred while deleting the brand.");
            }
        }
    }
}