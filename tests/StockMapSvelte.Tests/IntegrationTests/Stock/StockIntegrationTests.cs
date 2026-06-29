using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;
using StockMapSvelte.Domain.Entities;
using Xunit.Abstractions;

namespace StockMapSvelte.Tests.IntegrationTests.Stock;

[CollectionDefinition("IntegrationTests")]
public class SharedTestCollection : ICollectionFixture<IntegrationTestWebApplicationFactory> { }

[Collection("IntegrationTests")]
public class StockIntegrationTests : IAsyncLifetime
{
    private readonly IntegrationTestWebApplicationFactory _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly HttpClient _adminClient;
    private readonly HttpClient _managerClient;
    private readonly HttpClient _customerClient;
    private readonly HttpClient _anonymousClient;
    
    public StockIntegrationTests(IntegrationTestWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
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
    
    [Theory]
    [InlineData("GET", "/stocks/{id}")]
    [InlineData("GET", "/stocks")]
    [InlineData("PUT", "/stocks/{id}")]
    [InlineData("DELETE", "/stocks/{id}")]
    [InlineData("POST", "/stocks")]
    public async Task AdminOrManagerEndpoints_WithCustomerRole_ShouldReturnForbidden(string method, string urlTemplate)
    {
        // Arrange
        var requestUrl = urlTemplate.Replace("{id}", Guid.NewGuid().ToString());
        var request = new HttpRequestMessage(new HttpMethod(method), requestUrl);

        if (method is "POST" or "PUT")
        {
            request.Content = new StringContent("{}", Encoding.UTF8, "application/json");
        }

        // Act
        var response = await _customerClient.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    [Theory]
    [InlineData("GET", "/stocks/{id}")]
    [InlineData("GET", "/stocks")]
    [InlineData("PUT", "/stocks/{id}")]
    [InlineData("DELETE", "/stocks/{id}")]
    [InlineData("POST", "/stocks")]
    public async Task AdminOrManagerEndpoints_WithAdminRole_ShouldNotReturnForbidden(string method, string urlTemplate)
    {
        // Arrange
        var requestUrl = urlTemplate.Replace("{id}", Guid.NewGuid().ToString());
        var request = new HttpRequestMessage(new HttpMethod(method), requestUrl);

        if (method is "POST" or "PUT")
        {
            request.Content = new StringContent("{}", Encoding.UTF8, "application/json");
        }

        // Act
        var response = await _adminClient.SendAsync(request);

        // Assert
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("GET", "/stocks/{id}")]
    [InlineData("GET", "/stocks")]
    [InlineData("PUT", "/stocks/{id}")]
    [InlineData("DELETE", "/stocks/{id}")]
    [InlineData("POST", "/stocks")]
    public async Task AdminOrManagerEndpoints_WithManagerRole_ShouldNotReturnForbidden(string method, string urlTemplate)
    {
        // Arrange
        var requestUrl = urlTemplate.Replace("{id}", Guid.NewGuid().ToString());
        var request = new HttpRequestMessage(new HttpMethod(method), requestUrl);

        if (method is "POST" or "PUT")
        {
            request.Content = new StringContent("{}", Encoding.UTF8, "application/json");
        }

        // Act
        var response = await _managerClient.SendAsync(request);

        // Assert
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Theory]
    [InlineData("GET", "/stocks/{id}")]
    [InlineData("GET", "/stocks")]
    [InlineData("PUT", "/stocks/{id}")]
    [InlineData("DELETE", "/stocks/{id}")]
    [InlineData("POST", "/stocks")]
    [InlineData("GET", "/stocks/possible-to-add")]
    public async Task AllEndpoints_WithAnonymous_ShouldReturnUnauthorized(string method, string urlTemplate)
    {
        // Arrange
        var requestUrl = urlTemplate.Replace("{id}", Guid.NewGuid().ToString());
        var request = new HttpRequestMessage(new HttpMethod(method), requestUrl);

        if (method is "POST" or "PUT")
        {
            request.Content = new StringContent("{}", Encoding.UTF8, "application/json");
        }

        // Act
        var response = await _anonymousClient.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetPossibleToAddStocks_WithCustomerRole_ShouldNotReturnForbidden()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/stocks/possible-to-add");
        
        // Act
        var response = await _customerClient.SendAsync(request);

        // Assert
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    // CREATE STOCK

    [Fact]
    public async Task CreateStock_WithValidTicker_ShouldReturnCreated()
    {
        // Arrange
        var cmd = new CreateStockCommand("AAPL");

        // Act
        var response = await _adminClient.PostAsJsonAsync("/stocks", cmd);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var stock = await response.Content.ReadFromJsonAsync<StockDto>();
        Assert.NotNull(stock);
        Assert.Equal("AAPL", stock.TickerSymbol);
    }

    [Fact]
    public async Task CreateStock_WithEmptyTicker_ShouldReturnBadRequest()
    {
        // Arrange
        var cmd = new CreateStockCommand("");

        // Act
        var response = await _adminClient.PostAsJsonAsync("/stocks", cmd);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateStock_WithInvalidTicker_ShouldReturnBadRequest()
    {
        // Arrange
        var tickerSymbol = "INVALID";
        _factory.SetTickerExistsInStockClient(false, tickerSymbol);
        var cmd = new CreateStockCommand(tickerSymbol);

        // Act
        var response = await _adminClient.PostAsJsonAsync("/stocks", cmd);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateStock_WithDuplicateTicker_ShouldReturnConflict()
    {
        // Arrange
        var cmd = new CreateStockCommand("AAPL");
        await _adminClient.PostAsJsonAsync("/stocks", cmd);

        // Act
        var response = await _adminClient.PostAsJsonAsync("/stocks", cmd);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    // DELETE STOCK

    [Fact]
    public async Task DeleteStock_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var createResponse = await _adminClient.PostAsJsonAsync("/stocks", new CreateStockCommand("AAPL"));
        var stock = await createResponse.Content.ReadFromJsonAsync<StockDto>();
        Assert.NotNull(stock);

        // Act
        var response = await _adminClient.DeleteAsync($"/stocks/{stock.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteStock_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var response = await _adminClient.DeleteAsync($"/stocks/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // GET STOCK

    [Fact]
    public async Task GetStock_WithValidId_ShouldReturnStock()
    {
        // Arrange
        var createResponse = await _adminClient.PostAsJsonAsync("/stocks", new CreateStockCommand("AAPL"));
        var created = await createResponse.Content.ReadFromJsonAsync<StockDto>();
        Assert.NotNull(created);

        // Act
        var response = await _adminClient.GetAsync($"/stocks/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var stock = await response.Content.ReadFromJsonAsync<StockDto>();
        Assert.NotNull(stock);
        Assert.Equal("AAPL", stock.TickerSymbol);
    }

    [Fact]
    public async Task GetStock_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var response = await _adminClient.GetAsync($"/stocks/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // EDIT STOCK

    [Fact]
    public async Task EditStock_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var response = await _adminClient.PutAsync($"/stocks/{Guid.NewGuid()}?ticker=RANDOM", null);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task EditStock_WithExistentId_ShouldReturnNoContent()
    {
        // Act
        var createResponse = await _adminClient.PostAsJsonAsync("/stocks", new CreateStockCommand("AAPL"));
        
        var stock = await createResponse.Content.ReadFromJsonAsync<StockDto>();
        Assert.NotNull(stock);
        
        var response = await _adminClient.PutAsync($"/stocks/{stock.Id}?ticker=AMD", null);
        var content = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(content);
    
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task EditStock_NonExistentInStockClient_ShouldReturnBadRequest()
    {
        // Act
        var createResponse = await _adminClient.PostAsJsonAsync("/stocks", new CreateStockCommand("AAPL"));
        
        var stock = await createResponse.Content.ReadFromJsonAsync<StockDto>();
        Assert.NotNull(stock);
        
        var nonExistent = "NON-EXISTENT";
        _factory.SetTickerExistsInStockClient(false, nonExistent);
        
        var response = await _adminClient.PutAsync($"/stocks/{stock.Id}?ticker={nonExistent}", null);
    
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateStocksBatch_WithValidTickers_ShouldReturnOkWithCreatedAndNoFailed()
    {
        // Arrange
        var cmd = new CreateStocksCommand(["AAPL", "MSFT", "TSLA"]);

        // Act
        var response = await _adminClient.PostAsJsonAsync("/stocks/batch", cmd);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateStocksResponse>();
        Assert.NotNull(result);
        Assert.Equal(3, result.CreatedStocks .Length);
        Assert.Empty(result.FailedToCreateStocks );
    }

    [Fact]
    public async Task CreateStocksBatch_WithInvalidTickers_ShouldReturnOkWithFailedAndNoCreated()
    {
        // Arrange
        _factory.SetTickerExistsInStockClient(false);
        var cmd = new CreateStocksCommand(["INVALID1", "INVALID2"]);

        // Act
        var response = await _adminClient.PostAsJsonAsync("/stocks/batch", cmd);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateStocksResponse>();
        Assert.NotNull(result);
        Assert.Empty(result.CreatedStocks );
        Assert.Equal(2, result.FailedToCreateStocks .Length);
    }

    [Fact]
    public async Task CreateStocksBatch_WithMixedTickers_ShouldReturnOkWithSomeCreatedAndSomeFailed()
    {
        // Arrange
        _factory.SetTickerExistsInStockClient(true);
        _factory.SetTickerExistsInStockClient(false, "INVALID");
        var cmd = new CreateStocksCommand(["AAPL", "INVALID", "MSFT"]);

        // Act
        var response = await _adminClient.PostAsJsonAsync("/stocks/batch", cmd);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateStocksResponse>();
        Assert.NotNull(result);
        Assert.Equal(2, result.CreatedStocks .Length);
        Assert.Single(result.FailedToCreateStocks );
        Assert.Contains("INVALID", result.FailedToCreateStocks .Select(f => f.Ticker));
    }

    [Fact]
    public async Task CreateStocksBatch_WithDuplicateTickers_ShouldReturnOkWithFailedForDuplicates()
    {
        // Arrange - create AAPL first
        await _adminClient.PostAsJsonAsync("/stocks", new CreateStockCommand("AAPL"));
        var cmd = new CreateStocksCommand(["AAPL", "MSFT"]);

        // Act
        var response = await _adminClient.PostAsJsonAsync("/stocks/batch", cmd);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateStocksResponse>();
        Assert.NotNull(result);
        Assert.Single(result.CreatedStocks );
        Assert.Single(result.FailedToCreateStocks );
        Assert.Contains("AAPL", result.FailedToCreateStocks .Select(f => f.Ticker));
    }

    [Fact]
    public async Task CreateStocksBatch_WithEmptyList_ShouldReturnOkWithNothingCreated()
    {
        // Arrange
        var cmd = new CreateStocksCommand([]);

        // Act
        var response = await _adminClient.PostAsJsonAsync("/stocks/batch", cmd);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateStocksResponse>();
        Assert.NotNull(result);
        Assert.Empty(result.CreatedStocks );
        Assert.Empty(result.FailedToCreateStocks );
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();

        _factory.SetTickerExistsInStockClient(true);
        _factory.SetGetStockProfilesInStockClient([]);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}