using HttpSurge.UI.Models;
using Microsoft.EntityFrameworkCore;

namespace HttpSurge.UI.Data;

public class AppDbContext : DbContext
{
    public DbSet<TreeItem> TreeItems { get; set; }
    public DbSet<Collection> Collections { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<Api> Apis { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TreeItem>()
            .ToTable("TreeItems")
            .HasDiscriminator<string>("Type")
            .HasValue<Collection>("Collection")
            .HasValue<Folder>("Folder")
            .HasValue<Api>("Api");

        modelBuilder.Entity<TreeItem>()
            .HasOne(p => p.Parent)
            .WithMany(p => p.Children)
            .HasForeignKey(p => p.ParentId);
    }
}
