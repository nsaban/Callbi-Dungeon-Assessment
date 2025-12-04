using Dungeon.Infrastructure.Database.Configurations;
using Dungeon.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dungeon.Infrastructure.Database;

public class DungeonDbContext : DbContext
{
    public DungeonDbContext(DbContextOptions<DungeonDbContext> options) : base(options)
    {
    }

    public DbSet<DungeonMapEntity> DungeonMaps { get; set; }
    public DbSet<ObstacleEntity> Obstacles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new DungeonMapConfiguration());
        modelBuilder.ApplyConfiguration(new ObstacleConfiguration());
    }
}