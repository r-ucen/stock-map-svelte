using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Infrastructure.Database;
using StockMapSvelte.Infrastructure.Identity;
using Testcontainers.PostgreSql;

namespace StockMapSvelte.Tests.IntegrationTests;

public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public Mock<IStockClient> StockClientMock { get; } = new();
    
    private readonly PostgreSqlContainer _databaseContainer = new PostgreSqlBuilder("postgres:18")
        .WithDatabase("test")
        .WithUsername("admin")
        .WithPassword("admin")
        .Build();

    public async Task InitializeAsync()
    {
        await _databaseContainer.StartAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        await dbContext.Database.MigrateAsync(); 
    }

    public new async Task DisposeAsync()
    {
        await _databaseContainer.DisposeAsync();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings:DefaultConnection", _databaseContainer.GetConnectionString());
        
        builder.ConfigureTestServices(services =>
        {
            // SETUP IStockClient so that we dont call the real stock client
            // remove it
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IStockClient));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            // mock, set default behavior and register
            StockClientMock
                .Setup(x => x.TickerExists(It.IsAny<string>()))
                .ReturnsAsync(true);
            services.AddSingleton(StockClientMock.Object);
            
            services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, null);

            services.PostConfigureAll<OpenIdConnectOptions>(options =>
            {
                options.ClientId = "test-client-id";
            });
        });
    }
    
    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
        dbContext.Stocks.RemoveRange(dbContext.Stocks);
        dbContext.Portfolios.RemoveRange(dbContext.Portfolios);
        dbContext.StockProfiles.RemoveRange(dbContext.StockProfiles);
        dbContext.UserSettings.RemoveRange(dbContext.UserSettings);
        dbContext.Users.RemoveRange(dbContext.Users);
        
        await dbContext.SaveChangesAsync();
    }
    
    public void SetTickerExistsInStockClient(bool exists, string? ticker = null)
    {
        if (ticker is null)
        {
            StockClientMock
                .Setup(x => x.TickerExists(It.IsAny<string>()))
                .ReturnsAsync(exists);
        }
        else
        {
            StockClientMock
                .Setup(x => x.TickerExists(ticker))
                .ReturnsAsync(exists);
        }
    }
    
    public void SetGetStockProfilesInStockClient(List<StockProfile> profiles)
    {
        StockClientMock
            .Setup(x => x.GetStockProfilesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(profiles);
    }
    
    public async Task SeedStocksAsync(params string[] tickers)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var stocks = tickers.Select(t => new Domain.Entities.Stock
        {
            Id = new Guid(),
            TickerSymbol = t.ToUpperInvariant(),
            IsInitialized = true,
        });

        dbContext.Stocks.AddRange(stocks);
        await dbContext.SaveChangesAsync();
    }
    
    public async Task SeedUserAsync(string userId, string role = "Customer")
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var user = new ApplicationUser
        {
            Id = userId,
            UserName = $"{userId}@test.com",
            NormalizedUserName = $"{userId}@test.com".ToUpperInvariant(),
            Email = $"{userId}@test.com",
            NormalizedEmail = $"{userId}@test.com".ToUpperInvariant(),
            SecurityStamp = Guid.NewGuid().ToString()
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
    }
}