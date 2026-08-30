using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
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
using StockMapSvelte.Infrastructure.Cache;
using StockMapSvelte.Infrastructure.Identity;
using StockMapSvelte.Infrastructure.Repositories;
using StockMapSvelte.Infrastructure.Repositories.Cached;
using StockMapSvelte.Infrastructure.Services;
using StockMapSvelte.Infrastructure.Services.Trading212;
using StockMapSvelte.Infrastructure.Services.Yahoo;
using YahooQuotesApi;

namespace StockMapSvelte.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        string connectionString;

        if (string.IsNullOrEmpty(databaseUrl))
        {
            connectionString = configuration.GetConnectionString("DefaultConnection") 
                               ?? throw new InvalidOperationException("Connection string not found.");
        }
        else
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            
            connectionString = $"Host={databaseUri.Host};" +
                               $"Port={databaseUri.Port};" +
                               $"Database={databaseUri.LocalPath.TrimStart('/')};" +
                               $"Username={userInfo[0]};" +
                               $"Password={userInfo[1]};" +
                               $"SSL Mode=Require;" +
                               $"Trust Server Certificate=True;";
        }
        
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
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        services.AddScoped<StockRepository>();
        services.AddScoped<IStockRepository>(
            provider => new CachedStockRepository(
                provider.GetRequiredService<StockRepository>(),
                provider.GetRequiredService<HybridCache>()
            )
        );
        
        services.AddHttpClient("YahooSearchClient", client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
        });
        services.AddScoped<IStockClient, YahooStockClient>();
        
        services.AddScoped<PortfolioRepository>();
        services.AddScoped<IPortfolioRepository>(
            provider => new CachedPortfolioRepository(
                provider.GetRequiredService<PortfolioRepository>(),
                provider.GetRequiredService<HybridCache>()
            )
        );
        
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
        
        services.AddHttpClient<ITrading212Client, Trading212Client>();
        
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
            
            googleOptions.Events = new OpenIdConnectEvents
            {
                OnRemoteFailure = context =>
                {
                    var frontend = configuration["FrontendUrl"] ?? throw new InvalidOperationException("FrontendUrl not configured.");

                    context.Response.Redirect($"{frontend}/login");

                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };
        });
        
        var dataProtectionBuilder = services.AddDataProtection()
            .PersistKeysToDbContext<ApplicationDbContext>()
            .SetApplicationName("StockMapSvelte");
        
        var certBase64 = configuration["DataProtectionCert"];
        var certPassword = configuration["DataProtectionPassword"];

        if (!string.IsNullOrEmpty(certBase64))
        {
            var certBytes = Convert.FromBase64String(certBase64);
            var certificate = X509CertificateLoader.LoadPkcs12(
                certBytes,
                certPassword,
                X509KeyStorageFlags.EphemeralKeySet);
            
            dataProtectionBuilder.ProtectKeysWithCertificate(certificate);
        }

        services.AddHybridCache();
        services.AddScoped<ICacheService, HybridCacheService>();
        
        return services;
    }
}