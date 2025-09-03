using Microsoft.EntityFrameworkCore;
using Products.Catalogue.Application.Interfaces;
using Products.Catalogue.Domain.Entities;
using Products.Catalogue.Infrastructure.Data;
using SharedLibrary.Logs;
using SharedLibrary.Resonse;
using System.Linq.Expressions;

namespace Products.Catalogue.Infrastructure.Repositories
{
    public class CategoryRepository : ICategory// Make sure this interface exists
    {
        private readonly ProductDbContext _db;

        public CategoryRepository(ProductDbContext db)
        {
            _db = db;
        }

        public async Task<ApiRespose> AddAsync(Category entity)
        {
            try
            {
                var getCategory = await GetByAsync(c => c.Name!.Equals(entity.Name));
                if (getCategory is not null && !string.IsNullOrEmpty(entity.Name))
                {
                    return new ApiRespose(false, $"Category '{entity.Name}' already exists.");
                }

                await _db.Categories.AddAsync(entity);
                await _db.SaveChangesAsync();

                if (entity.Id > 0)
                {
                    return new ApiRespose(true, $"Category '{entity.Name}' added successfully.", entity);
                }
                else
                {
                    return new ApiRespose(false, $"Failed to add '{entity.Name}' category.");
                }
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                return new ApiRespose(false, "An error occurred while adding the category.");
            }
        }

        public async Task<ApiRespose> FindIdAsync(int id)
        {
            try
            {
                var category = await _db.Categories.FindAsync(id);
                if (category is not null)
                {
                    return new ApiRespose(true, $"Category with ID {id} retrieved successfully.", category);
                }
                else
                {
                    return new ApiRespose(false, $"Category with ID {id} does not exist.");
                }
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                return new ApiRespose(false, $"An error occurred while finding category with ID {id}.");
            }
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            try
            {
                return await _db.Categories.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                throw new InvalidOperationException("An error occurred while retrieving categories.");
            }
        }

        public async Task<Category> GetByAsync(Expression<Func<Category, bool>> predicate)
        {
            try
            {
                return await _db.Categories.Where(predicate).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                throw new InvalidOperationException("An error occurred while retrieving a category.");
            }
        }

        public async Task<ApiRespose> UpdateAsync(Category entity)
        {
            try
            {
                var existingCategory = await _db.Categories.FindAsync(entity.Id);
                if (existingCategory is null)
                {
                    return new ApiRespose(false, $"Category with ID {entity.Id} does not exist.");
                }

                _db.Entry(existingCategory).CurrentValues.SetValues(entity);
                await _db.SaveChangesAsync();

                return new ApiRespose(true, $"Category with ID {entity.Id} updated successfully.", entity);
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                return new ApiRespose(false, "An error occurred while updating the category.");
            }
        }

        public async Task<ApiRespose> DeleteAsync(Category entity)
        {
            try
            {
                // First, we need to find the entity in the database using its ID
                // to ensure we are deleting an entity that is being tracked by Entity Framework.
                var categoryToDelete = await _db.Categories.FindAsync(entity.Id);

                // Check if the entity was actually found in the database
                if (categoryToDelete is null)
                {
                    return new ApiRespose(false, $"Category with ID {entity.Id} does not exist.");
                }

                // If found, remove it from the context
                _db.Categories.Remove(categoryToDelete);

                // Save the changes to the database
                await _db.SaveChangesAsync();

                // Return a success response
                return new ApiRespose(true, $"Category with ID {entity.Id} deleted successfully.");
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                return new ApiRespose(false, "An error occurred while deleting the category.");
            }
        }
    }
}