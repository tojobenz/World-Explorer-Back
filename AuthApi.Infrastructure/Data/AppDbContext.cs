using AuthApi.Domain.Entities;
using AuthApi.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace AuthApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Auth Entities
    public DbSet<User> Users { get; set; }

    // Wikimedia Entities
    public DbSet<WikimediaPlace> WikimediaPlaces { get; set; }
    public DbSet<WikimediaPerson> WikimediaPeople { get; set; }
    public DbSet<WikimediaMonument> WikimediaMonuments { get; set; }
    public DbSet<WikimediaFact> WikimediaFacts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new WikimediaPlaceConfiguration());
        modelBuilder.ApplyConfiguration(new WikimediaPersonConfiguration());
        modelBuilder.ApplyConfiguration(new WikimediaMonumentConfiguration());
        modelBuilder.ApplyConfiguration(new WikimediaFactConfiguration());
    }

    public async Task SeedDataAsync()
    {
        // Check if admin user already exists
        var adminUser = await Users.FirstOrDefaultAsync(u => u.Email == "admin@test.com");
        
        if (adminUser == null)
        {
            // Create default admin user
            var newAdmin = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin1234*"),
                FirstName = "Admin",
                LastName = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            await Users.AddAsync(newAdmin);
            await SaveChangesAsync();
        }
    }
}