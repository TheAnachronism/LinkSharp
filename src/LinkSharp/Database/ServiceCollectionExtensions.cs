using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LinkSharp.Database;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLinkSharpIdentity(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(
            o => o.UseNpgsql(configuration.GetConnectionString("LinkSharpDatabase"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(o =>
        {
            o.Cookie.Name = "LinkSharpAuth";
            o.ExpireTimeSpan = TimeSpan.FromMinutes(10);
            o.Cookie.SameSite = SameSiteMode.Strict;
            o.Cookie.HttpOnly = true;
            o.Cookie.SecurePolicy = CookieSecurePolicy.Always;

            o.SlidingExpiration = true;
            o.LoginPath = "/login";
            o.LogoutPath = "/logout";

            o.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
        });
        
        return services;
    }
}