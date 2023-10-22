using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;

namespace IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRazorPages();

            builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.EmitStaticAudienceClaim = true;
            })
            .AddTestUsers(Config.Users)
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryApiResources(Config.ApiResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryIdentityResources(Config.IdentityResources);


            var app = builder.Build();

            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapRazorPages().RequireAuthorization();

            app.Run();
        }
    }
}