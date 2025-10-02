using GameOfLife.Models;
using Microsoft.EntityFrameworkCore;

namespace GameOfLife.Persistence;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    public DbSet<BoardEntity> Boards { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BoardEntity>().HasKey(b => b.Id);
        modelBuilder.Entity<BoardEntity>()
            .Property(b => b.Stability)
            .HasConversion<string>();
    }
}