using GP.Lib.Base.DataLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GP.Lib.Repo.Entity_Configurations
{
    public class DbGeoPointsConfiguration : IEntityTypeConfiguration<DbGeoPoints>
    {
        public void Configure(EntityTypeBuilder<DbGeoPoints> builder)
        {
            builder.Property(t => t.UserId).IsRequired();
            builder.Property(t => t.OriginLat).IsRequired();
            builder.Property(t => t.OriginLon).IsRequired();
            builder.Property(t => t.DestinationLat).IsRequired();
            builder.Property(t => t.DestinationLon).IsRequired();
        }
    }
}