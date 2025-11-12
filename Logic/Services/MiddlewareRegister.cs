using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Logic.Helpers;
using Logic.IHelpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;


public static class MiddlewareRegister
{
    public static IServiceCollection RegisterHelpers(this IServiceCollection services)
    {
        services.AddScoped<IUserHelper, UserHelper>();
        return services;
    }
    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var user = context.GetHttpContext().User;
            return user != null && user.Identity.IsAuthenticated && user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "SuperAdmin");
        }
    }

    public static IApplicationBuilder UseHangfireConfiguration(this IApplicationBuilder app)
    {
        var options = new BackgroundJobServerOptions
        {
            ServerName = $"{Environment.MachineName}.{Guid.NewGuid()}"
        };

        app.UseHangfireServer(options);

        var robotStorage = new SqlServerStorage(app.ApplicationServices.GetService<IConfiguration>().GetConnectionString("DBConnectionHangFire"));
        JobStorage.Current = robotStorage;

        var dashboardOptions = new DashboardOptions
        {
            Authorization = new[] { new MyAuthorizationFilter() }
        };
        app.UseHangfireDashboard("/ApparusHangFire", dashboardOptions, robotStorage);

        return app;
    }
}

