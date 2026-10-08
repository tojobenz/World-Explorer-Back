using AuthApi.Domain.Entities;
using AuthApi.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

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
}