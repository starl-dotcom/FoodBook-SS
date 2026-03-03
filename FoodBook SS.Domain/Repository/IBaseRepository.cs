using System.Linq.Expressions;
using FoodBook_SS.Domain.Base;

namespace FoodBook_SS.Domain.Repository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {

        Task<OperationResult> SaveEntityAsync(TEntity entity);

        Task<OperationResult> UpdateEntityAsync(TEntity entity);

        Task<OperationResult> GetAllAsync(Expression<Func<TEntity, bool>> filter);

        Task<List<TEntity>> GetAllAsync();

        Task<TEntity?> GetEntityByIdAsync(int id);

        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);

    }
}
