using CalendrierAventCore.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using NLog;

namespace CalendrierAventCore.Data;

public class DefaultDbContext : DbContext
{
    public DbSet<Box> Boxes { get; set; }
    public DbSet<Calendar> Calendars { get; set; }
    public DbSet<Picture> Pictures { get; set; }



    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
    {
    }
    public DefaultDbContext()
    {
        // Method intentionally left empty.
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        const string development = "Development";
        const string sqlite = "Sqlite";
        const string sqlServer = "SqlServer";

        var path = Directory.GetCurrentDirectory();
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? development;
        var repository = env == development ? sqlite : sqlServer;

        _logger.Info($"env is {env}, Repository is {repository}");
        
        var builder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile($"appsettings.{env}.json");

        var config = builder.Build();
        //var connectionString = config.GetConnectionString(repository);
        var connectionString = "Server=51.178.46.82,1533;Database=calendrierdelavent;User Id=calendrierdelavent;Password=2jp5uieg!C69T";
        //if (repository == sqlServer) optionsBuilder.UseSqlServer(connectionString);
        //else optionsBuilder.UseSqlite(connectionString);
        optionsBuilder.UseSqlServer(connectionString);
    }
}