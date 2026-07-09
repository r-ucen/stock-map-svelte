using Moq;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Handlers;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Domain.Exceptions.Portfolio;

namespace StockMapSvelte.Tests.UnitTests.UseCases.PortfolioUseCases;

public class CreatePortfolioTests
{
    [Fact]
    public async Task CreatePortfolioHandle_Should_Save_Portfolio_And_Return_Populated_Dto()
    {
        // Arrange
        var portfolioRepoMock = new Mock<IPortfolioRepository>();
        var userSettingRepoMock = new Mock<IUserSettingRepository>();
        var userContextMock = new Mock<IUserContext>();
        var stockRepoMock = new Mock<IStockRepository>();
        
        var userId = "test-user-id";
        
        // mock user context to return a fake user id
        userContextMock.Setup(x => x.GetCurrentUserIdAsync()).ReturnsAsync(userId);
        
        // mock portfolio repository to return that the portfolio name does not exist (false)
        portfolioRepoMock.Setup(r => r.PortfolioNameExistsAsync(userId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        // return 1 on portfolio creation
        portfolioRepoMock.Setup(r => r.CreatePortfolioAsync(It.IsAny<Portfolio>(), It.IsAny<IList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        // assume we already have one portfolio, so that the set default checks will be skipped
        portfolioRepoMock.Setup(r => r.GetPortfolioCountByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        
        stockRepoMock.Setup(r => r.GetUninitializedStocks(It.IsAny<IList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>());
        
        var useCase = new CreatePortfolioHandler(userContextMock.Object, portfolioRepoMock.Object, userSettingRepoMock.Object, stockRepoMock.Object);
        
        var cmd = new CreatePortfolioCommand
        (
            "Test Portfolio",
            new List<string> { "AAPL", "MSFT" }
        );
        
        // Act
        var result = await useCase.Handle(cmd, CancellationToken.None);
        
        // Assert
        portfolioRepoMock.Verify(r =>
                r.CreatePortfolioAsync(
                    It.Is<Portfolio>(p =>
                        p.Name == "Test Portfolio" &&
                        p.UserId == userId
                    ),
                    It.Is<List<string>>(l =>
                        l.Contains("AAPL") &&
                        l.Contains("MSFT")
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once);
        
        Assert.NotEqual(Guid.Empty, result.PortfolioId);
        
        Assert.Equal("Test Portfolio", result.PortfolioName);
    }
    
    [Fact]
    public async Task CreatePortfolioHandle_Should_Throw_PortfolioNameMissingException()
    {
        // Arrange
        var portfolioRepoMock = new Mock<IPortfolioRepository>();
        var userSettingRepoMock = new Mock<IUserSettingRepository>();
        var userContextMock = new Mock<IUserContext>();
        var stockRepoMock = new Mock<IStockRepository>();
        
        var userId = "test-user-id";
        
        // mock user context to return a fake user id
        userContextMock.Setup(x => x.GetCurrentUserIdAsync()).ReturnsAsync(userId);
        
        // mock portfolio repository to return that the portfolio name does not exist (false)
        portfolioRepoMock.Setup(r => r.PortfolioNameExistsAsync(userId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        // return 1 on portfolio creation
        portfolioRepoMock.Setup(r => r.CreatePortfolioAsync(It.IsAny<Portfolio>(), It.IsAny<IList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        // assume we already have one portfolio, so that the set default checks will be skipped
        portfolioRepoMock.Setup(r => r.GetPortfolioCountByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        
        stockRepoMock.Setup(r => r.GetUninitializedStocks(It.IsAny<IList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>());
        
        var useCase = new CreatePortfolioHandler(userContextMock.Object, portfolioRepoMock.Object, userSettingRepoMock.Object, stockRepoMock.Object);
        
        var cmd = new CreatePortfolioCommand
        (
            "",
            new List<string> { "AAPL", "MSFT" }
        );
        
        // Act
        await Assert.ThrowsAsync<PortfolioNameMissingException>(
            () => useCase.Handle(cmd, CancellationToken.None)
        );
    }
    
    [Fact]
    public async Task CreatePortfolioHandle_Should_Throw_PortfolioNameAlreadyExistsException()
    {
        // Arrange
        var portfolioRepoMock = new Mock<IPortfolioRepository>();
        var userSettingRepoMock = new Mock<IUserSettingRepository>();
        var userContextMock = new Mock<IUserContext>();
        var stockRepoMock = new Mock<IStockRepository>();
        
        var userId = "test-user-id";
        
        // mock user context to return a fake user id
        userContextMock.Setup(x => x.GetCurrentUserIdAsync()).ReturnsAsync(userId);
        
        // mock portfolio repository to return that the portfolio name does exist (true)
        portfolioRepoMock.Setup(r => r.PortfolioNameExistsAsync(userId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        // return 1 on portfolio creation
        portfolioRepoMock.Setup(r => r.CreatePortfolioAsync(It.IsAny<Portfolio>(), It.IsAny<IList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        // assume we already have one portfolio, so that the set default checks will be skipped
        portfolioRepoMock.Setup(r => r.GetPortfolioCountByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        
        stockRepoMock.Setup(r => r.GetUninitializedStocks(It.IsAny<IList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>());
        
        var useCase = new CreatePortfolioHandler(userContextMock.Object, portfolioRepoMock.Object, userSettingRepoMock.Object, stockRepoMock.Object);
        
        var cmd = new CreatePortfolioCommand
        (
            "Test Portfolio",
            new List<string> { "AAPL", "MSFT" }
        );
        
        // Act
        await Assert.ThrowsAsync<PortfolioNameAlreadyExistsException>(
            () => useCase.Handle(cmd, CancellationToken.None)
        );
    }
    
    [Fact]
    public async Task CreatePortfolioHandle_Should_Save_Portfolio_And_Should_Set_As_Default_And_Return_Populated_Dto()
    {
        // Arrange
        var portfolioRepoMock = new Mock<IPortfolioRepository>();
        var userSettingRepoMock = new Mock<IUserSettingRepository>();
        var userContextMock = new Mock<IUserContext>();
        var stockRepoMock = new Mock<IStockRepository>();
        
        var userId = "test-user-id";
        
        // mock user context to return a fake user id
        userContextMock.Setup(x => x.GetCurrentUserIdAsync()).ReturnsAsync(userId);
        
        // mock portfolio repository to return that the portfolio name does not exist (false)
        portfolioRepoMock.Setup(r => r.PortfolioNameExistsAsync(userId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        // return 1 on portfolio creation
        portfolioRepoMock.Setup(r => r.CreatePortfolioAsync(It.IsAny<Portfolio>(), It.IsAny<IList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        // this is the first portfolio, so the setting as default logic will be triggered
        portfolioRepoMock.Setup(r => r.GetPortfolioCountByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        // return 1 on set portfolio as default
        userSettingRepoMock.Setup(r => r.SetPortfolioAsDefaultAsync(userId, It.IsAny<Guid>()))
            .ReturnsAsync(1);
        
        stockRepoMock.Setup(r => r.GetUninitializedStocks(It.IsAny<IList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>());
        
        var useCase = new CreatePortfolioHandler(userContextMock.Object, portfolioRepoMock.Object, userSettingRepoMock.Object, stockRepoMock.Object);
        
        var cmd = new CreatePortfolioCommand
        (
            "Test Portfolio",
            new List<string> { "AAPL", "MSFT" }
        );
        
        // Act
        var result = await useCase.Handle(cmd, CancellationToken.None);
        
        // Assert
        userSettingRepoMock.Verify(r => r.SetPortfolioAsDefaultAsync(userId, It.IsAny<Guid>()), Times.Once);
        
        portfolioRepoMock.Verify(r =>
                r.CreatePortfolioAsync(
                    It.Is<Portfolio>(p =>
                        p.Name == "Test Portfolio" &&
                        p.UserId == userId
                    ),
                    It.Is<List<string>>(l =>
                        l.Contains("AAPL") &&
                        l.Contains("MSFT")
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once);
        
        Assert.NotEqual(Guid.Empty, result.PortfolioId);
        
        Assert.Equal("Test Portfolio", result.PortfolioName);
    }
    
    [Fact]
    public async Task CreatePortfolioHandle_Should_Throw_PortfolioCreationFailedException_Set_As_Default_Fail()
    {
        // Arrange
        var portfolioRepoMock = new Mock<IPortfolioRepository>();
        var userSettingRepoMock = new Mock<IUserSettingRepository>();
        var userContextMock = new Mock<IUserContext>();
        var stockRepoMock = new Mock<IStockRepository>();
        
        var userId = "test-user-id";
        
        // mock user context to return a fake user id
        userContextMock.Setup(x => x.GetCurrentUserIdAsync()).ReturnsAsync(userId);
        
        // mock portfolio repository to return that the portfolio name does not exist (false)
        portfolioRepoMock.Setup(r => r.PortfolioNameExistsAsync(userId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        // return 1 on portfolio creation
        portfolioRepoMock.Setup(r => r.CreatePortfolioAsync(It.IsAny<Portfolio>(), It.IsAny<IList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        // this is the first portfolio, so the setting as default logic will be triggered
        portfolioRepoMock.Setup(r => r.GetPortfolioCountByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        // return 0 on set portfolio as default (not successful)
        userSettingRepoMock.Setup(r => r.SetPortfolioAsDefaultAsync(userId, It.IsAny<Guid>()))
            .ReturnsAsync(0);
        
        stockRepoMock.Setup(r => r.GetUninitializedStocks(It.IsAny<IList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>());
        
        var useCase = new CreatePortfolioHandler(userContextMock.Object, portfolioRepoMock.Object, userSettingRepoMock.Object, stockRepoMock.Object);
        
        var cmd = new CreatePortfolioCommand
        (
            "Test Portfolio",
            new List<string> { "AAPL", "MSFT" }
        );
        
        // Act
        await Assert.ThrowsAsync<PortfolioCreationFailedException>(() => useCase.Handle(cmd, CancellationToken.None));
    }
    
    [Fact]
    public async Task CreatePortfolioHandle_Should_Throw_PortfolioCreationFailedException_Create_Fail()
    {
        // Arrange
        var portfolioRepoMock = new Mock<IPortfolioRepository>();
        var userSettingRepoMock = new Mock<IUserSettingRepository>();
        var userContextMock = new Mock<IUserContext>();
        var stockRepoMock = new Mock<IStockRepository>();
        
        var userId = "test-user-id";
        
        // mock user context to return a fake user id
        userContextMock.Setup(x => x.GetCurrentUserIdAsync()).ReturnsAsync(userId);
        
        // mock portfolio repository to return that the portfolio name does not exist (false)
        portfolioRepoMock.Setup(r => r.PortfolioNameExistsAsync(userId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        // return 0 on portfolio creation (fail)
        portfolioRepoMock.Setup(r => r.CreatePortfolioAsync(It.IsAny<Portfolio>(), It.IsAny<IList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        
        // this is the first portfolio, so the setting as default logic will be triggered
        portfolioRepoMock.Setup(r => r.GetPortfolioCountByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        // return 1 on set portfolio as default (successful)
        userSettingRepoMock.Setup(r => r.SetPortfolioAsDefaultAsync(userId, It.IsAny<Guid>()))
            .ReturnsAsync(1);
        
        stockRepoMock.Setup(r => r.GetUninitializedStocks(It.IsAny<IList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>());
        
        var useCase = new CreatePortfolioHandler(userContextMock.Object, portfolioRepoMock.Object, userSettingRepoMock.Object, stockRepoMock.Object);
        
        var cmd = new CreatePortfolioCommand
        (
            "Test Portfolio",
            new List<string> { "AAPL", "MSFT" }
        );
        
        // Act
        await Assert.ThrowsAsync<PortfolioCreationFailedException>(() => useCase.Handle(cmd, CancellationToken.None));
    }
}