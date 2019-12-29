using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using GP.Lib.Base.DataLayer;
using GP.Lib.Common.Constants;

namespace GP.Lib.Repo.Entity_Configurations
{
    public class DbUserConfiguration : IEntityTypeConfiguration<DbUser>
    {
        private readonly IPasswordHasher<DbUser> _passwordHasher;

        public DbUserConfiguration(IPasswordHasher<DbUser> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public void Configure(EntityTypeBuilder<DbUser> builder)
        {
            builder.Property(n => n.FirstName).HasMaxLength(128).IsRequired();
            builder.Property(n => n.LastName).HasMaxLength(128).IsRequired();
            

            var User = new DbUser
            {
                Id = 1,
                UserName = ConstantApp.SuperUser,
                NormalizedUserName = ConstantApp.SuperUser.ToUpper(),
                FirstName = "Super",
                LastName = "User",
                Email = "user@test.com",
                NormalizedEmail = "USER@TEST.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            User.PasswordHash = _passwordHasher.HashPassword(User, "Password123$");
            
            builder.HasData(User);
        }
    }
}