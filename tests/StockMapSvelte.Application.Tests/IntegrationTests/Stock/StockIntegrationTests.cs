using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;

namespace StockMapSvelte.Application.Tests.IntegrationTests.Stock;

[CollectionDefinition("StockIntegrationTests")]
public class SharedTestCollection : ICollectionFixture<IntegrationTestWebApplicationFactory> { }

[Collection("StockIntegrationTests")]
public class StockIntegrationTests
{
    private readonly IntegrationTestWebApplicationFactory _factory;
    
    private readonly HttpClient _adminClient;
    private readonly HttpClient _managerClient;
    private readonly HttpClient _customerClient;
    private readonly HttpClient _anonymousClient;
    
    public StockIntegrationTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
        
        var clientOptions = new WebApplicationFactoryClientOptions { AllowAutoRedirect = false };
        
        _adminClient = factory.CreateClient(clientOptions);
        _adminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
        _adminClient.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeaderName, "Admin");

        _managerClient = factory.CreateClient(clientOptions);
        _managerClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
        _managerClient.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeaderName, "Manager");

        _customerClient = factory.CreateClient(clientOptions);
        _customerClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
        _customerClient.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeaderName, "Customer");

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
}