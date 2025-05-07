using System;
using Microsoft.EntityFrameworkCore;
using DBConnection.Entity;

namespace DBConnection;

public class AppDbContext : DbContext
{
    public DbSet<User> User {get; set;}
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("core_user");
            entity.HasKey(e => e.GuidId);
            
            entity.HasIndex(u => u.Login)
                .IsUnique();
                
            entity.Property(e => e.RevokedBy)
                .IsRequired(false);
                
            entity.Property(e => e.RevokedOn)
                .IsRequired(false);
                
            // Настройка для enum
            entity.Property(e => e.Gender)
                .HasConversion<int>();
        });
    }
}
