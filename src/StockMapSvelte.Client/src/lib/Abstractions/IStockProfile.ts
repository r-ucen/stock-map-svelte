export interface IStockProfile {
    tickerSymbol?: string;
    date?: string;
    fullName?: string;
    sector?: string;
    currency?: string;
    regularMarketChangePercent?: number;
    regularMarketPrice?: number;
    earningsDate?: string;
    dividendDate?: string;
    exDividendDate?: string;
    dividendYield?: number;
    beta?: number;
    pe?: number;
    forwardPe?: number;
    shortRatio?: number;
    analystRecommendationMean?: number;
    analystRecommendationKey?: string;
    volume?: number;
    profitMargins?: number;
    earningsQuarterlyGrowth?: number;
    trailingEps?: number;
    forwardEps?: number;
    pegRatio?: number;
    oneYearChange?: number;
    targetHighPrice?: number;
    targetLowPrice?: number;
    targetMeanPrice?: number;
    targetMedianPrice?: number;
    totalDebt?: number;
    freeCashflow?: number;
    earningsGrowth?: number;
    revenueGrowth?: number;
}

