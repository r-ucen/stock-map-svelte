using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Tests.IntegrationTests.UserSetting;

[Collection("IntegrationTests")]
public class UserSettingIntegrationTests : IAsyncLifetime
{
    private readonly IntegrationTestWebApplicationFactory _factory;
    
    private readonly HttpClient _customerClient;

    public UserSettingIntegrationTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
        
        var clientOptions = new WebApplicationFactoryClientOptions { AllowAutoRedirect = false };
        
        _customerClient = factory.CreateClient(clientOptions);
        _customerClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
        _customerClient.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeaderName, "Customer");
        _customerClient.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeaderName, "customer-user-id-1");
    }

    [Fact]
    public async Task GetDefaultPortfolioId_ShouldReturnDefaultPortfolioId()
    {
        // Arrange
        var response = await _customerClient.GetAsync("/portfolios/me");
        var result = await response.Content.ReadFromJsonAsync<List<PortfolioDto>>();
        
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        Assert.True(result.Count < 2);
        var portfolio = result[0];
        Assert.True(portfolio.IsDefault);

        // Act
        
        var defaultPortfolioIdResponse = await _customerClient.GetAsync("/user-settings/default-portfolio");
        var defaultPortfolioId = await defaultPortfolioIdResponse.Content.ReadFromJsonAsync<Guid>();
        
        // Assert
        Assert.Equal(portfolio.PortfolioId,  defaultPortfolioId);
    }
    
    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        await _factory.SeedUserAsync("customer-user-id-1");
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}