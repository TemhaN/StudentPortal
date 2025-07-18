using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using StudentPortal.Data;
using System;

namespace StudentPortal.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            try
            {
                Console.WriteLine("AppDbContextFactory: Starting CreateDbContext");

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                Console.WriteLine($"AppDbContextFactory: BasePath = {System.AppDomain.CurrentDomain.BaseDirectory}");

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json.");
                }

                Console.WriteLine($"AppDbContextFactory: ConnectionString = {connectionString}");

                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                Console.WriteLine("AppDbContextFactory: Creating AppDbContext");
                return new AppDbContext(optionsBuilder.Options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AppDbContextFactory: Error = {ex.Message}");
                throw;
            }
        }
    }
}