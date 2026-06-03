namespace StockMapSvelte.Application.UseCases.StockUseCases.Commands;

public record EditStockCommand(Guid Id, string TickerSymbol);