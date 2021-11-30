using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace CalendrierAventCore.Data
{
    public class DefaultDesignTimeDbContextFactory : IDesignTimeDbContextFactory<DefaultDbContext>
    {
        public DefaultDbContext CreateDbContext(string[] args)
        {
            const string development = "Development";
            const string sqlite = "Sqlite";
            const string sqlServer = "SqlServer";
            DbContextOptionsBuilder<DefaultDbContext> optionsBuilder = new DbContextOptionsBuilder<DefaultDbContext>();
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? development;
            var repository = env == development ? sqlite : sqlServer;

            Console.WriteLine($"env is {env}, Repository is {repository}");
            var path = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile($"appsettings.{env}.json");

            var config = builder.Build();
            //var connectionString = config.GetConnectionString(repository);
            var connectionString = "Server=51.178.46.82,1533;Database=calendrierdelavent;User Id=calendrierdelavent;Password=2jp5uieg!C69T";
            //if (repository == sqlServer) optionsBuilder.UseSqlServer(connectionString);
            //else optionsBuilder.UseSqlite(connectionString);
            optionsBuilder.UseSqlServer(connectionString);

            return new DefaultDbContext(optionsBuilder.Options);
        }
    }
}