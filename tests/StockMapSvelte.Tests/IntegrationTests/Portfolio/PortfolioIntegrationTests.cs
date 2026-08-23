using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using StockMapSvelte.Api.Requests;
using StockMapSvelte.Api.Requests.Portfolio;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;

namespace StockMapSvelte.Tests.IntegrationTests.Portfolio;


[Collection("IntegrationTests")]
public class PortfolioIntegrationTests : IAsyncLifetime
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    private readonly HttpClient _adminClient;
    private readonly HttpClient _managerClient;
    private readonly HttpClient _customerClient;
    private readonly HttpClient _anonymousClient;
    
    public PortfolioIntegrationTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;

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
    
    [Fact]
    public async Task CreatePortfolio_WithGivenStocks_ShouldContainTheStocks()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL", "MSFT", "GOOGL", "AMD");
        var portfolioToCreate = new CreatePortfolioRequest("Name1", ["AAPL",  "MSFT", "GOOGL", "AMD"]);

        // Act
        var response = await _customerClient.PostAsJsonAsync("/portfolios", portfolioToCreate);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<PortfolioDto>();
        Assert.NotNull(result);
        Assert.Equal("Name1", result.Name);
        Assert.Contains("AAPL", result.TickerSymbols);
        Assert.Contains("MSFT", result.TickerSymbols);
        Assert.Contains("GOOGL", result.TickerSymbols);
        Assert.Contains("AMD", result.TickerSymbols);
    }
    
    // DELETE
    
    [Fact]
    public async Task DeletePortfolio_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL");
        var portfolioToCreate = new CreatePortfolioRequest("Name", ["AAPL"]);
        var createResponse = await _customerClient.PostAsJsonAsync("/portfolios", portfolioToCreate);
        var createdPortfolio = await createResponse.Content.ReadFromJsonAsync<PortfolioDto>();
        Assert.NotNull(createdPortfolio);
        
        // Act
        var response = await _customerClient.DeleteAsync($"portfolios/{createdPortfolio.Id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _customerClient.GetAsync($"portfolios/{createdPortfolio.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
    
    [Fact]
    public async Task DeletePortfolio_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _customerClient.DeleteAsync($"portfolios/{Guid.NewGuid()}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task DeletePortfolio_WithOtherUsersPortfolioId_ShouldReturnForbidden()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL");
        var portfolioToCreate = new CreatePortfolioRequest("Name", ["AAPL"]);
        var createResponse = await _managerClient.PostAsJsonAsync("/portfolios", portfolioToCreate);
        var createdPortfolio = await createResponse.Content.ReadFromJsonAsync<PortfolioDto>();
        Assert.NotNull(createdPortfolio);
        
        // Act
        var response = await _customerClient.DeleteAsync($"portfolios/{createdPortfolio.Id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    // EDIT

    [Fact]
    public async Task EditPortfolio_WithValidData_ShouldReturnOk()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL", "MSFT");
        var createResponse = await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Original Name", ["AAPL"]));
        var createdPortfolio = await createResponse.Content.ReadFromJsonAsync<PortfolioDto>();
        Assert.NotNull(createdPortfolio);

        var editRequest = new EditPortfolioRequest("Updated Name", ["AAPL", "MSFT"]);

        // Act
        var response = await _customerClient.PutAsJsonAsync($"/portfolios/{createdPortfolio.Id}", editRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PortfolioDto>();
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Contains("MSFT", result.TickerSymbols);
    }

    [Fact]
    public async Task EditPortfolio_WithoutName_ShouldReturnBadRequest()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL");
        var createResponse = await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Original Name", ["AAPL"]));
        var createdPortfolio = await createResponse.Content.ReadFromJsonAsync<PortfolioDto>();
        Assert.NotNull(createdPortfolio);

        var editRequest = new EditPortfolioRequest("", ["AAPL"]);

        // Act
        var response = await _customerClient.PutAsJsonAsync($"/portfolios/{createdPortfolio.Id}", editRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task EditPortfolio_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var editRequest = new EditPortfolioRequest("Updated Name", []);

        // Act
        var response = await _customerClient.PutAsJsonAsync($"/portfolios/{Guid.NewGuid()}", editRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task EditPortfolio_WithDuplicateName_ShouldReturnConflict()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL");
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("First Portfolio", ["AAPL"]));
        var createResponse = await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Second Portfolio", ["AAPL"]));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        
        var secondPortfolio = await createResponse.Content.ReadFromJsonAsync<PortfolioDto>();
        Assert.NotNull(secondPortfolio);

        var editRequest = new EditPortfolioRequest("First Portfolio", ["AAPL"]);

        // Act
        var response = await _customerClient.PutAsJsonAsync($"/portfolios/{secondPortfolio.Id}", editRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task EditPortfolio_WithUnchangedData_ShouldReturnConflict()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL");
        var createResponse = await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Original Name", ["AAPL"]));
        var createdPortfolio = await createResponse.Content.ReadFromJsonAsync<PortfolioDto>();
        Assert.NotNull(createdPortfolio);

        var editRequest = new EditPortfolioRequest("Original Name", ["AAPL"]);

        // Act
        var response = await _customerClient.PutAsJsonAsync($"/portfolios/{createdPortfolio.Id}", editRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task EditPortfolio_WithOtherUsersPortfolioId_ShouldReturnForbidden()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL", "MSFT");
        var createResponse = await _managerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Manager Portfolio", ["AAPL"]));
        var managerPortfolio = await createResponse.Content.ReadFromJsonAsync<PortfolioDto>();
        Assert.NotNull(managerPortfolio);

        var editRequest = new EditPortfolioRequest("Other User Portfolio Name", ["MSFT"]);

        // Act
        var response = await _customerClient.PutAsJsonAsync($"/portfolios/{managerPortfolio.Id}", editRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    // GET PORTFOLIOS BY USER ID

    [Fact]
    public async Task GetPortfoliosByUserId_WithNoPortfolios_ShouldReturnDefaultPortfolio()
    {
        // Act
        var response = await _customerClient.GetAsync("/portfolios/me");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<PortfolioDto>>();
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Default portfolio", result[0].Name);
    }

    [Fact]
    public async Task GetPortfoliosByUserId_WithNoPortfolios_DefaultPortfolioShouldBeSetAsDefault()
    {
        // Act
        var response = await _customerClient.GetAsync("/portfolios/me");
        var result = await response.Content.ReadFromJsonAsync<List<PortfolioDto>>();

        // Assert
        Assert.NotNull(result);
        Assert.True(result[0].IsDefault);
    }

    [Fact]
    public async Task GetPortfoliosByUserId_WithExistingPortfolios_ShouldReturnAll()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL", "MSFT");
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("First Portfolio", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Second Portfolio", ["MSFT"]));

        // Act
        var response = await _customerClient.GetAsync("/portfolios/me");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<PortfolioDto>>();
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetPortfoliosByUserId_FirstCreatedPortfolio_ShouldBeSetAsDefault()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL", "MSFT");
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("First Portfolio", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Second Portfolio", ["MSFT"]));

        // Act
        var response = await _customerClient.GetAsync("/portfolios/me");
        var result = await response.Content.ReadFromJsonAsync<List<PortfolioDto>>();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result, p => p.IsDefault);
    }

    [Fact]
    public async Task GetPortfoliosByUserId_ShouldOnlyReturnOwnPortfolios()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL");
        await _managerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Manager Portfolio", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Customer Portfolio", ["AAPL"]));

        // Act
        var response = await _customerClient.GetAsync("/portfolios/me");
        var result = await response.Content.ReadFromJsonAsync<List<PortfolioDto>>();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Customer Portfolio", result[0].Name);
    }

    [Fact]
    public async Task GetPortfoliosByUserId_CalledTwiceWithNoPortfolios_ShouldNotCreateMultipleDefaultPortfolios()
    {
        // Act
        await _customerClient.GetAsync("/portfolios/me");
        var response = await _customerClient.GetAsync("/portfolios/me");
        var result = await response.Content.ReadFromJsonAsync<List<PortfolioDto>>();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }
    
    // OTHER
    
    [Fact]
    public async Task CreateThreePortfolios_FirstShouldBeDefault()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL");
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name1", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name2", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name3", ["AAPL"]));

        // Act
        var portfolioCount = await _customerClient.GetAsync("/portfolios/me");
        var result = await portfolioCount.Content.ReadFromJsonAsync<List<PortfolioDto>>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        
        var defaultPortfolio = result.FirstOrDefault(p => p.IsDefault);
        Assert.NotNull(defaultPortfolio);
        Assert.Equal("Name1", defaultPortfolio.Name);
        Assert.True(defaultPortfolio.IsDefault);
    }
    
    [Fact]
    public async Task CreateFivePortfolios_ShouldHaveFivePortfolios()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL");
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name1", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name2", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name3", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name4", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name5", ["AAPL"]));

        // Act
        var portfolioCount = await _customerClient.GetAsync("/portfolios/me");
        var result = await portfolioCount.Content.ReadFromJsonAsync<List<PortfolioDto>>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
    }
    
    [Fact]
    public async Task CreateFivePortfolios_WithTwoValid_ShouldHaveTwoPortfolios()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL");
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name1", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name2", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name2", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name2", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name2", ["AAPL"]));

        // Act
        var portfolioCount = await _customerClient.GetAsync("/portfolios/me");
        var result = await portfolioCount.Content.ReadFromJsonAsync<List<PortfolioDto>>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }
    
    [Fact]
    public async Task CreateSixPortfolios_HoweverExceedingLimitOfFive_ShouldHaveFivePortfolios()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL");
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name1", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name2", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name3", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name4", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name5", ["AAPL"]));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Name6", ["AAPL"]));

        // Act
        var portfolioCount = await _customerClient.GetAsync("/portfolios/me");
        var result = await portfolioCount.Content.ReadFromJsonAsync<List<PortfolioDto>>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
    }
    
    // GET ALL
    
    [Fact]
    public async Task GetAllPortfolios_ShouldReturnAll()
    {
        // Arrange
        await _factory.SeedStocksAsync("AAPL", "MSFT", "AMD", "AMAT");

        var stocks1 = new List<string>() { "AAPL" }.OrderBy(s => s).ToList();
        var stocks2 = new List<string>() { "MSFT", "AAPL", "AMD" }.OrderBy(s => s).ToList();
        var stocks3 = new List<string>() { "AAPL", "AMD" }.OrderBy(s => s).ToList();

        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("First Portfolio", stocks1));
        await _customerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Second Portfolio", stocks2));
        await _managerClient.PostAsJsonAsync("/portfolios", new CreatePortfolioRequest("Third Portfolio", stocks3));

        // Act
        var response = await _adminClient.GetAsync("/portfolios/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<PortfolioDto>>();
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalRecords);
        
        var firstPortfolio = result.Data.FirstOrDefault(p => p.Name == "First Portfolio");
        var secondPortfolio = result.Data.FirstOrDefault(p => p.Name == "Second Portfolio");
        var thirdPortfolio = result.Data.FirstOrDefault(p => p.Name == "Third Portfolio");
        
        Assert.NotNull(firstPortfolio);
        Assert.NotNull(secondPortfolio);
        Assert.NotNull(thirdPortfolio);
        
        Assert.Equal("First Portfolio", firstPortfolio.Name);
        Assert.Equal("Second Portfolio", secondPortfolio.Name);
        Assert.Equal("Third Portfolio", thirdPortfolio.Name);
        
        Assert.Equal(stocks1, firstPortfolio.TickerSymbols.OrderBy(s => s).ToList());
        Assert.Equal(stocks2, secondPortfolio.TickerSymbols.OrderBy(s => s).ToList());
        Assert.Equal(stocks3, thirdPortfolio.TickerSymbols.OrderBy(s => s).ToList());
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