import type { Component } from 'svelte';

export interface IState {
	portfolios: IPortfolio[];
	portfolioLogoDefault: Component;
	selectedPortfolioId: string | null;
	// selectedMetric: string | null;
	// isToastAutoHide: boolean | null;
	// autoHideDelayMs: number | null;
}

export interface IPortfolio {
	portfolioId: string;
	userId: string;
	portfolioName: string;
	isDefault: boolean;
	tickerSymbols: string[];
}
	