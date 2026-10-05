using AuthApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuthApi.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=AuthApiDb;Trusted_Connection=true;MultipleActiveResultSets=true");
        
        return new AppDbContext(optionsBuilder.Options);
    }
}