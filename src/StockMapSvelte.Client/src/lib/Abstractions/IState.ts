import type { Component } from 'svelte';
import type { TreemapDataDto } from '$lib/Abstractions/Treemap';

export interface IState {
	email: string | undefined;
	portfolios: IPortfolio[];
	portfolioLogoDefault: Component;
	selectedPortfolioId: string | null;
	selectedMetric: MapMetric | null;
	// isToastAutoHide: boolean | null;
	// autoHideDelayMs: number | null;
	treemapData: TreemapDataDto | null;
}

export interface IPortfolio {
	portfolioId: string;
	userId: string;
	portfolioName: string;
	isDefault: boolean;
	tickerSymbols: string[];
}

export enum MapMetric {
	RegularMarketChangePercent,
	PreMarketChangePercent,
	PostMarketChangePercent,
	MarketState,
	Volume,
	DividendDate,
	ExDividendDate,
	DividendYield,
	EarningsDate,
	Beta,
	Pe,
	ForwardPe,
	ShortRatio,
	AnalystRecommendation,
	profitMargins,
	earningsQuarterlyGrowth,
	trailingEps,
}

export function getMetricLabel(metric: MapMetric): string {
	const labels: Record<MapMetric, string> = {
		[MapMetric.RegularMarketChangePercent]: "Market Change (%)",
		[MapMetric.PreMarketChangePercent]: "Pre-Market Change (%)",
		[MapMetric.PostMarketChangePercent]: "Post-Market Change (%)",
		[MapMetric.MarketState]: "Market State",
		[MapMetric.Volume]: "Trading Volume",
		[MapMetric.DividendDate]: "Dividend Date",
		[MapMetric.ExDividendDate]: "Ex-Dividend Date",
		[MapMetric.DividendYield]: "Dividend Yield",
		[MapMetric.EarningsDate]: "Earnings Date",
		[MapMetric.Beta]: "Beta",
		[MapMetric.Pe]: "P/E Ratio",
		[MapMetric.ForwardPe]: "Forward P/E",
		[MapMetric.ShortRatio]: "Short Ratio",
		[MapMetric.AnalystRecommendation]: "Analyst Recommendation",
		[MapMetric.profitMargins]: "Profit Margins",
		[MapMetric.earningsQuarterlyGrowth]: "Earnings Quarterly Growth",
		[MapMetric.trailingEps]: "EPS",
	};
	return labels[metric] ?? "Unknown Metric";
}