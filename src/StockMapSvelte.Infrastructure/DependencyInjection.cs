using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StockMapSvelte.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Resend;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.Services;
using StockMapSvelte.Infrastructure.BackgroundServices;
using StockMapSvelte.Infrastructure.Identity;
using StockMapSvelte.Infrastructure.Repositories;
using StockMapSvelte.Infrastructure.Services;
using YahooQuotesApi;

namespace StockMapSvelte.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddScoped(p => 
            p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
        
        services.AddIdentityApiEndpoints<ApplicationUser>(options => {
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.None;
            
            // disable redirect to login page for API calls, return 401 instead
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
        });
        
        // Resend
        services.AddOptions<ResendClientOptions>()
            .Bind(configuration.GetSection("Resend"));
        services.AddHttpClient<IResend, ResendClient>();
        services.AddTransient<IEmailSender<ApplicationUser>, ResendEmailSender>();
        
        // DI
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IStockUpdateService, StockUpdateService>();
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<IStockClient, YahooStockClient>();
        services.AddScoped<IPortfolioRepository, PortfolioRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITreeMapRepository, TreeMapRepository>();
        services.AddScoped<IStockProfileRepository, StockProfileRepository>();
        services.AddScoped<IUserSettingRepository, UserSettingRepository>();
        
        services.AddHostedService<StockDataUpdateTimedService>();
        services.AddSingleton<YahooQuotes>(new YahooQuotesBuilder().Build());
        
        // Authorization policies
        services.AddAuthorizationBuilder()
            // every authenticated user is a customer
            .AddPolicy("IsCustomer", policy =>
            {
                policy.RequireAuthenticatedUser();
            });
        
        return services;
    }
}