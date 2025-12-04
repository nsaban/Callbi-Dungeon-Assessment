using Dungeon.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dungeon.Infrastructure.Database.Configurations;

public class ObstacleConfiguration : IEntityTypeConfiguration<ObstacleEntity>
{
    public void Configure(EntityTypeBuilder<ObstacleEntity> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.X)
            .IsRequired();
            
        builder.Property(e => e.Y)
            .IsRequired();
            
        builder.Property(e => e.DungeonMapId)
            .IsRequired();
            
        // Foreign key relationship
        builder.HasOne(e => e.DungeonMap)
            .WithMany(m => m.Obstacles)
            .HasForeignKey(e => e.DungeonMapId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Index for performance on coordinate queries
        builder.HasIndex(e => new { e.X, e.Y, e.DungeonMapId })
            .HasDatabaseName("IX_Obstacles_Coordinates_MapId");
    }
}