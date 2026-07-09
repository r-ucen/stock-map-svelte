using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
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
using StockMapSvelte.Infrastructure.Repositories.Cached;
using StockMapSvelte.Infrastructure.Services;
using YahooQuotesApi;

namespace StockMapSvelte.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddIdentityApiEndpoints<ApplicationUser>(options => {
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        
        // Resend
        services.AddOptions<ResendClientOptions>()
            .Bind(configuration.GetSection("Resend"));
        services.AddHttpClient<IResend, ResendClient>();
        services.AddTransient<IEmailSender<ApplicationUser>, ResendEmailSender>();
        
        // DI
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IStockUpdateService, StockUpdateService>();
        
        services.AddScoped<StockRepository>();
        services.AddScoped<IStockRepository>(
            provider => new CachedStockRepository(
                provider.GetRequiredService<StockRepository>(),
                provider.GetRequiredService<HybridCache>()
            )
        );
        
        services.AddScoped<IStockClient, YahooStockClient>();
        
        services.AddScoped<PortfolioRepository>();
        services.AddScoped<IPortfolioRepository>(
            provider => new CachedPortfolioRepository(
                provider.GetRequiredService<PortfolioRepository>(),
                provider.GetRequiredService<HybridCache>()
            )
        );
        
        services.AddScoped<IUserRepository, UserRepository>();
        
        services.AddScoped<TreeMapRepository>();
        services.AddScoped<ITreeMapRepository>(
            provider => new CachedTreeMapRepository(
                provider.GetRequiredService<TreeMapRepository>(),
                provider.GetRequiredService<HybridCache>()
            )
        );
        
        services.AddScoped<IStockProfileRepository, StockProfileRepository>();
        services.AddScoped<IUserSettingRepository, UserSettingRepository>();
        
        services.AddHostedService<StockDataUpdateTimedService>();
        services.AddSingleton<YahooQuotes>(new YahooQuotesBuilder().Build());
        services.AddSingleton<ITreeMapUpdateNotifier, TreeMapUpdateNotifier>();
        
        // Authorization policies
        services.AddAuthorizationBuilder()
            // every authenticated user is a customer
            .AddPolicy("IsCustomer", policy =>
            {
                policy.RequireAuthenticatedUser();
            });
        
        services.AddAuthentication().AddGoogleOpenIdConnect(googleOptions =>
        {
            googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
            googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
            googleOptions.SignInScheme = IdentityConstants.ExternalScheme; 
        });
        
        services.AddHybridCache();
        
        return services;
    }
}