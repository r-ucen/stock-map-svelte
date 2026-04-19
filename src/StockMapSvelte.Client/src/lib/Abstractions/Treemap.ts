export interface RectangleDto {
	x: number;
	y: number;
	width: number;
	height: number;
}

export interface TreemapNodeDto {
	tickerSymbol: string;
	sector: string;
	fullName: string;
	marketCap: number;
	rectangle: RectangleDto;
	regularMarketChangePercent?: number;
	regularMarketPrice?: number;
	preMarketChangePercent?: number;
	preMarketPrice?: number;
	postMarketChangePercent?: number;
	postMarketPrice?: number;
	marketState?: string;
	currency?: string;
	volume?: number;
	dividendDate?: string;
	exDividendDate?: string;
	earningsDate?: string;
	dividendYield?: number;
	beta?: number;
	pe?: number;
	forwardPe?: number;
	shortRatio?: number;
	analystRecommendationMean?: number;
	analystRecommendationKey?: string;
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

export interface TreemapSectorDto {
	sectorName: string;
	totalMarketCap: number;
	stocks: TreemapNodeDto[];
	rectangle: RectangleDto;
}

export interface TreemapDataDto {
	sectors: TreemapSectorDto[];
	totalMarketCap: number;
}
