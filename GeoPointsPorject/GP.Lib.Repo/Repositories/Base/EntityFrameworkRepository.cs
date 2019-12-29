using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GP.Lib.Base.DataLayer.Base;
using GP.Lib.Base.Interfaces.Repositories.Base;

namespace GP.Lib.Repo.Repositories.Base
{
    public class EntityFrameworkRepository<TEntity> : EntityFrameworkReadOnlyRepository<TEntity>, IRepository<TEntity> where TEntity : class
    {
        #region Ctor
        public EntityFrameworkRepository(DbContext context)
            : base(context)
        {
        }
        #endregion

        #region IRepository
        public virtual void Create(TEntity entity, string createdBy = null)
        {
            var e = entity as Entity;
            if (e != null)
            {
                e.CreatedAt = DateTime.UtcNow;
                e.CreatedBy = createdBy;
            }
            Context.Set<TEntity>().Add(entity);
        }
        public virtual void Update(TEntity entity, string modifiedBy = null)
        {
            var e = entity as Entity;
            if (e != null)
            {
                e.ModifiedAt = DateTime.UtcNow;
                e.ModifiedBy = modifiedBy;
            }
            Context.Set<TEntity>().Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }
        public virtual void Delete(int id)
        {
            var entity = Context.Set<TEntity>().Find(id);
            Delete(entity);
        }
        public virtual void Delete(TEntity entity)
        {
            var dbSet = Context.Set<TEntity>();
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }
        public void Save() => Context.SaveChanges();
        public TEntity Find(int id) => Context.Set<TEntity>().Find(id);
        public IEnumerable<TEntity> Get() => Context.Set<TEntity>();
        public void Dispose() => Context.Dispose();
        public async Task<TEntity> FindAsync(int entityId) => await Context.Set<TEntity>().FindAsync(entityId);

        public async Task CreateAsync(TEntity entity)
        {
            var e = entity as Entity;
            if (e != null)
            {
                e.CreatedAt = DateTime.UtcNow;
            }
            await Context.Set<TEntity>().AddAsync(entity);
        }
        public Task UpdateAsync(TEntity entity)
        {
            var e = entity as Entity;
            if (e != null)
            {
                e.ModifiedAt = DateTime.UtcNow;
            }

            Context.Set<TEntity>().Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;

            return Task.FromResult(0);
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await FindAsync(id);
            await DeleteAsync(entity);
        }
        public Task DeleteAsync(TEntity entity)
        {
            var dbSet = Context.Set<TEntity>();
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
            return Task.FromResult(0);
        }
        public Task SaveAsync() => Context.SaveChangesAsync();
        #endregion
    }
}