using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityServer.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Program).Assembly.GetName().Name;

            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(connectionString, opt => opt.MigrationsAssembly(migrationsAssembly));
            });
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.EmitStaticAudienceClaim = true;
            })
            .AddConfigurationStore(options => options.ConfigureDbContext = b => b.UseSqlite(connectionString, opt => opt.MigrationsAssembly(migrationsAssembly)))
            .AddOperationalStore(options => options.ConfigureDbContext = b => b.UseSqlite(connectionString, opt => opt.MigrationsAssembly(migrationsAssembly)))
            .AddAspNetIdentity<IdentityUser>();

            var app = builder.Build();

            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapRazorPages().RequireAuthorization();


            var seed = args.Contains("/seed");
            if (seed)
            {
                args = args.Except(new[] { "/seed" }).ToArray();
            }

            if (seed)
            {
                SeedData.EnsureSeedData(connectionString);
                return;
            }

            app.Run();
        }
    }
}