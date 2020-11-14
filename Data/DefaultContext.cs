using CalendrierAventCore.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace CalendrierAventCore.Data
{
    public class DefaultContext : DbContext
    {
        public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
        {
        }
        public DefaultContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            string path = Directory.GetCurrentDirectory();
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            string persistance = Environment.GetEnvironmentVariable("PERSISTANCE") ?? "Sqlite";
            string dataBase;
            if (persistance == "SqlServer")
                dataBase = "AdminDbContext";
            else
                dataBase = "SqliteDbContext";

            IConfigurationBuilder builder = new ConfigurationBuilder()
                               .SetBasePath(path)
                               .AddJsonFile($"appsettings.{env}.json");
            IConfigurationRoot config = builder.Build();
            string connectionString = config.GetConnectionString(dataBase);
            Console.WriteLine($"ASPNETCORE_ENVIRONMENT is : {env}");
            Console.WriteLine($"PERSISTANCE is : {persistance} ; connectionString is : {connectionString}");

            if (persistance == "SqlServer")
                optionsBuilder.UseSqlServer(connectionString);
            else if (persistance == "Sqlite")
                optionsBuilder.UseSqlite(connectionString);
        }

        #region Properties
        public DbSet<Box> Boxes { get; set; }
        public DbSet<Calendar> Calendars { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        #endregion
    }
}
