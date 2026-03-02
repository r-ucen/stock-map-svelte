using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StockMapSvelte.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Resend;
using StockMapSvelte.Infrastructure.Identity;
using StockMapSvelte.Infrastructure.Services;

namespace StockMapSvelte.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddAuthorizationBuilder();
        
        services.AddIdentityApiEndpoints<ApplicationUser>(options => {
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        
        services.AddOptions<ResendClientOptions>()
            .Bind(configuration.GetSection("Resend"));
        services.AddHttpClient<IResend, ResendClient>();
        services.AddTransient<IEmailSender<ApplicationUser>, ResendEmailSender>();
        
        return services;
    }
}