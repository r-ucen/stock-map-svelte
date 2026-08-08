using StockMapSvelte.Infrastructure.Services;

namespace StockMapSvelte.Tests.UnitTests;

public class StockUpdateJitterTests
{
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteTrue_OnSaturdayAt_00_00()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 8, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Saturday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.True(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteTrue_OnSaturdayAt_01_00()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 8, 1, 0, 0, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Saturday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.True(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteTrue_OnSaturdayAt_01_59()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 8, 1, 59, 59, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Saturday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.True(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteFalse_OnSaturdayAt_02_00()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 8, 2, 0, 0, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Saturday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.False(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteFalse_OnSaturdayAt_22_00()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 8, 22, 0, 0, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Saturday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.False(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteFalse_OnSundayAt_00_00()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 9, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Sunday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.False(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteFalse_OnSundayAt_01_59()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 9, 1, 59, 59, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Sunday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.False(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteFalse_OnSundayAt_22_00()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 9, 22, 0, 0, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Sunday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.False(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteFalse_OnMondayAt_00_00()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 10, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Monday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.False(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteFalse_OnMondayAt_01_59()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 10, 1, 59, 59, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Monday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.False(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteTrue_OnMondayAt_02_00()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 10, 2, 0, 0, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Monday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.True(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteTrue_OnMondayAt_10_00()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 10, 10, 0, 0, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Monday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.True(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteTrue_OnMondayAt_23_59()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 10, 23, 59, 59, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Monday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.True(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteFalse_OnTuesdayAt_00_00()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 11, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Tuesday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.False(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteFalse_OnTuesdayAt_03_00()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 11, 3, 0, 0, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Tuesday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.False(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteFalse_OnTuesdayAt_05_59()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 11, 5, 59, 0, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Tuesday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.False(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteTrue_OnTuesdayAt_06_00()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 11, 6, 0, 0, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Tuesday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.True(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteTrue_OnTuesdayAt_23_59()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 11, 23, 59, 59, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Tuesday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.True(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteFalse_OnWednesdayAt_23_59()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 12, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Wednesday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.False(shouldExecute);
    }
    
    [Fact]
    public async Task StockUpdateJitter_ShouldReturnExecuteTrue_OnFridayAt_23_59()
    {
        // Arrange
        var isFirstRun = false;
        
        var date = new DateTimeOffset(2026, 8, 14, 23, 59, 59, TimeSpan.Zero);
        Assert.Equal(DayOfWeek.Friday, date.DayOfWeek);
        
        // Act
        var (shouldExecute, _) = StockUpdateJitter.GetVariableDelayInMinutes(date, ref isFirstRun);
        
        // Assert
        Assert.True(shouldExecute);
    }
}