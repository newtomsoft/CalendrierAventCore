using System;
using CalendrierAventCore.Data;
using Data.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CalendrierAventCore;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();
        //var connectionString = Configuration.GetConnectionString("SqlServer");
        var connectionString = "Server=51.178.46.82,1533;Database=calendrierdelavent;User Id=calendrierdelavent;Password=2jp5uieg!C69T";
        services.AddDbContext<DefaultDbContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Scoped);
        services.AddMvc(options => options.EnableEndpointRouting = false)
                .AddRazorPagesOptions(o => o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute())); ;
        services.AddOptions();
        services.Configure<MyConfig>(Configuration.GetSection("MyConfig"));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseMvc(routes =>
        {
            routes.MapRoute(
                name: "AjoutCalendrier",
                template: "/",
                defaults: new { controller = "Calendrier", action = "Ajouter" }
            );
            routes.MapRoute(
                name: "DirectCalendrier",
                template: "{name}",
                defaults: new { controller = "Calendrier", action = "Lire" }
            );
            routes.MapRoute(
                name: "DirectModifier",
                template: "Modifier/{name}",
                defaults: new { controller = "Calendrier", action = "Modifier" }
            );
            routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
        });
    }
}