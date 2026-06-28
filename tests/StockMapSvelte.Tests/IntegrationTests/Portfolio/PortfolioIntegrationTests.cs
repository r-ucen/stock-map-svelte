using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using StockMapSvelte.Api.Requests;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;
using Xunit.Abstractions;

namespace StockMapSvelte.Tests.IntegrationTests.Portfolio;


[Collection("IntegrationTests")]
public class PortfolioIntegrationTests : IAsyncLifetime
{
    private readonly IntegrationTestWebApplicationFactory _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly HttpClient _adminClient;
    private readonly HttpClient _managerClient;
    private readonly HttpClient _customerClient;
    private readonly HttpClient _anonymousClient;
    
    public PortfolioIntegrationTests(IntegrationTestWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;

        var clientOptions = new WebApplicationFactoryClientOptions { AllowAutoRedirect = false };
        
        _adminClient = factory.CreateClient(clientOptions);
        _adminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
        _adminClient.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeaderName, "Admin");
        _adminClient.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeaderName, "admin-user-id-1");

        _managerClient = factory.CreateClient(clientOptions);
        _managerClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
        _managerClient.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeaderName, "Manager");
        _managerClient.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeaderName, "manager-user-id-1");


        _customerClient = factory.CreateClient(clientOptions);
        _customerClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
        _customerClient.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeaderName, "Customer");
        _customerClient.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeaderName, "customer-user-id-1");

        _anonymousClient = factory.CreateClient(clientOptions);
        _anonymousClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
        _anonymousClient.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeaderName, "Anonymous");
    }
    
    // CREATE
    [Fact]
    public async Task CreatePortfolio_WithoutName_ShouldReturnBadRequest()
    {
        // Arrange
        var portfolioToCreate = new CreatePortfolioRequest("", []);

        // Act
        var response = await _customerClient.PostAsJsonAsync("/portfolios", portfolioToCreate);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task CreatePortfolio_WithValidName_ShouldReturnCreated()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL");
        var portfolioToCreate = new CreatePortfolioRequest("Name", ["AAPL"]);

        // Act
        var response = await _customerClient.PostAsJsonAsync("/portfolios", portfolioToCreate);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    
    [Fact]
    public async Task CreatePortfolio_WithDuplicateName_ShouldReturnConflict()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL");
        var portfolioToCreate = new CreatePortfolioRequest("Name", ["AAPL"]);
        await _customerClient.PostAsJsonAsync("/portfolios", portfolioToCreate);

        // Act
        var response = await _customerClient.PostAsJsonAsync("/portfolios", portfolioToCreate);
        
        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    
    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        
        await _factory.SeedUserAsync("admin-user-id-1", "Admin");
        await _factory.SeedUserAsync("manager-user-id-1", "Manager");
        await _factory.SeedUserAsync("customer-user-id-1");
        
        _factory.SetTickerExistsInStockClient(true);
        _factory.SetGetStockProfilesInStockClient([]);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}