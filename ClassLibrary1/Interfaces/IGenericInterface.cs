using SharedLibrary.Resonse;
using System.Linq.Expressions;

namespace Products.Catalogue.Application.Interfaces
{
    public interface IGenericInterface<T> where T : class
    {
        Task<ApiRespose> AddAsync(T entity);
        Task<ApiRespose> UpdateAsync(T entity);
        Task<ApiRespose> DeleteAsync(T entity);
        Task<ApiRespose> FindIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByAsync(Expression<Func<T, bool>> predicate);
    }
}
