using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DebugMeBackend.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection is not configured.");

            string databaseProvider = configuration["DatabaseProvider"] ?? "MySql";

            DbContextOptionsBuilder<AppDbContext> optionsBuilder = new();

            if (databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                optionsBuilder.UseSqlite(connectionString);
            }
            else
            {
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
