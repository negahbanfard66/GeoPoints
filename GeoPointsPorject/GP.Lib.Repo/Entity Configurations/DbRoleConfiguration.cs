using GP.Lib.Base.DataLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GP.Lib.Repo.Entity_Configurations
{
    public class DbRoleConfiguration : IEntityTypeConfiguration<DbRole>
    {
        public void Configure(EntityTypeBuilder<DbRole> builder)
        {
            builder.HasData(
                new DbRole { Id = 1, Name = "user", NormalizedName = "USER" });
        }
    }
}