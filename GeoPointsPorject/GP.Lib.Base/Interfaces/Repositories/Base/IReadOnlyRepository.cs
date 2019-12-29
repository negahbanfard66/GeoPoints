using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GP.Lib.Base.Interfaces.Repositories.Base
{
    public interface IReadOnlyRepository<TEntity>
    {
        Task<IQueryable<TEntity>> GetAllAsync(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null);

        Task<IQueryable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null);

        Task<TEntity> GetOneAsync(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null);

        Task<TEntity> GetFirstAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null);


        IQueryable<TEntity> GetAll(string includeProperties = null);

        IQueryable<TEntity> GetAllNoTracking(string includeProperties = null);

        Task<IQueryable<TEntity>> GetQueryableAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null);
    }
}
