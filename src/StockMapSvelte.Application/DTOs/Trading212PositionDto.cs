    namespace StockMapSvelte.Application.DTOs;

    public record Trading212InstrumentDto(
        string Currency,
        string Isin,
        string Name,
        string Ticker
    );

    public record Trading212WalletImpactDto(
        string Currency,
        decimal CurrentValue,
        decimal FxImpact,
        decimal TotalCost,
        decimal UnrealizedProfitLoss
    );

    public record Trading212PositionDto(
        decimal AveragePricePaid,
        DateTime CreatedAt,
        decimal CurrentPrice,
        Trading212InstrumentDto Instrument,
        decimal Quantity,
        decimal QuantityAvailableForTrading,
        decimal QuantityInPies,
        Trading212WalletImpactDto WalletImpact
    );