using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GP.Lib.Base.Interfaces.Repositories.Base
{
    public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>, IDisposable
    {
        TEntity Find(int entity);
        Task<TEntity> FindAsync(int entityId);
        IEnumerable<TEntity> Get();
        void Create(TEntity entity, string createdBy);
        Task CreateAsync(TEntity entity);
        void Update(TEntity entity, string modifiedBy);
        Task UpdateAsync(TEntity entity);
        void Delete(int id);
        Task DeleteAsync(int id);
        void Delete(TEntity entity);
        Task DeleteAsync(TEntity entity);
        void Save();
        Task SaveAsync();
    }
}
