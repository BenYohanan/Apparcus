using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Logic;
using Logic.Helpers;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;


public static class MiddlewareRegister
{
    public static IServiceCollection RegisterHelpers(this IServiceCollection services)
    {
        services.AddScoped<IUserHelper, UserHelper>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<IProjectHelper, ProjectHelper>();
        services.AddScoped<IDropdownHelper, DropdownHelper>();
        return services;
    }
    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var user = context.GetHttpContext().User;
            return user != null && user.Identity.IsAuthenticated && user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
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
        AppHttpContext.Services = app.ApplicationServices;
        app.UseHangfireDashboard("/ApparusHangFire", dashboardOptions, robotStorage);

        return app;
    }
}

