using Microsoft.EntityFrameworkCore;
using MyApp.Models.Entities;

namespace MyApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(200).IsRequired();

            entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("ix_users_email");

            entity
                .Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
