using HttpSurge.UI.Models;
using Microsoft.EntityFrameworkCore;

namespace HttpSurge.UI.Data;

public class AppDbContext : DbContext
{
    public DbSet<TreeItem> TreeItems { get; set; }
    public DbSet<Collection> Collections { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<Api> Apis { get; set; }
    public DbSet<Header> Headers { get; set; }
    public DbSet<QueryParam> QueryParams { get; set; }

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

        modelBuilder.Entity<Api>()
            .HasMany(a => a.Headers)
            .WithOne(h => h.Api)
            .HasForeignKey(h => h.ApiId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Api>()
            .HasMany(a => a.QueryParams)
            .WithOne(q => q.Api)
            .HasForeignKey(q => q.ApiId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
