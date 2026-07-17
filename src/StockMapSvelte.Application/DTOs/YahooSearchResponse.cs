using System.Text.Json.Serialization;

namespace StockMapSvelte.Application.DTOs;

public record YahooSearchResponse(
    [property: JsonPropertyName("count")] int Count,
    [property: JsonPropertyName("quotes")] YahooSearchQuote[] Quotes
);

public record YahooSearchQuote(
    [property: JsonPropertyName("symbol")] string Symbol,
    [property: JsonPropertyName("shortname")] string? ShortName,
    [property: JsonPropertyName("longname")] string? LongName,
    [property: JsonPropertyName("exchange")] string? Exchange,
    [property: JsonPropertyName("quoteType")] string? QuoteType,
    [property: JsonPropertyName("sector")] string? Sector,
    [property: JsonPropertyName("industry")] string? Industry
);