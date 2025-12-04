using Dungeon.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dungeon.Infrastructure.Database.Configurations;

public class DungeonMapConfiguration : IEntityTypeConfiguration<DungeonMapEntity>
{
    public void Configure(EntityTypeBuilder<DungeonMapEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(x => x.Width)
            .IsRequired();
        
        builder.Property(x => x.Height)
            .IsRequired();
        
        builder.Property(x => x.StartX)
            .IsRequired();
            
        builder.Property(x => x.StartY)
            .IsRequired();
            
        builder.Property(x => x.GoalX)
            .IsRequired();
            
        builder.Property(x => x.GoalY)
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}