using Library.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
{
    public LibraryDbContext CreateDbContext(string[] args)
    {
        // Use the default configuration (e.g., appsettings.json) to build the connection string
        var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();

        // Create a configuration builder and set the path to your appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())  // Current directory where the migration command is run
            .AddJsonFile("appsettings.json")
            .Build();

        // Use the connection string from the configuration
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("LibraryConnectionString"));

        return new LibraryDbContext(optionsBuilder.Options);
    }
}
