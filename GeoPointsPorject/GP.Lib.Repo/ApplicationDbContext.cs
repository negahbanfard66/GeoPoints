using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GP.Lib.Base.DataLayer;
using GP.Lib.Repo.Entity_Configurations;
using GP.Lib.Base.Interfaces.Repositories.Base;

namespace GP.Lib.Repo
{
    public class ApplicationDbContext : IdentityDbContext<DbUser, DbRole, int>
    {
        private readonly IHttpContextAccessor _contextAccessor;

        private void HandleModifiedEntities()
        {
            var modifiedEntries = from entry in ChangeTracker.Entries()
                                  where entry.State == EntityState.Added || entry.State == EntityState.Modified
                                  select entry;

            foreach (var entry in modifiedEntries)
            {
                if (entry.Entity is IEntity changedOrAddedItem)
                {
                    if (entry.State == EntityState.Added)
                    {
                        changedOrAddedItem.CreatedAt = DateTime.UtcNow;
                        changedOrAddedItem.CreatedBy = _contextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
                    }
                    else
                    {
                        changedOrAddedItem.ModifiedAt = DateTime.UtcNow;
                        changedOrAddedItem.ModifiedBy = _contextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
                    }
                }
            }
        }

        #region CTOR
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor contextAccessor)
            : base(options)
        {
            _contextAccessor = contextAccessor;
        }
        #endregion

        #region Properties
        public DbSet<DbGeoPoints> GeoPoints { get; set; }
        #endregion


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new DbUserConfiguration(new PasswordHasher<DbUser>()));
            builder.ApplyConfiguration(new DbRoleConfiguration());
            builder.ApplyConfiguration(new UserRoleConfiguration());
            builder.ApplyConfiguration(new DbGeoPointsConfiguration());

            builder.Entity<IdentityUserRole<Guid>>().HasKey(p => new { p.UserId, p.RoleId });
        }

        #region Save Overrides

        public override int SaveChanges()
        {
            HandleModifiedEntities();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            HandleModifiedEntities();

            return base.SaveChangesAsync(cancellationToken);
        }

        #endregion
    }
}
