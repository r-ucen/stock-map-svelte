import { MapMetric } from '$lib/Abstractions/IState';
import type {
	RectangleDto,
	TreemapDataDto,
	TreemapNodeDto,
	TreemapSectorDto
} from '$lib/Abstractions/Treemap';
import * as ColorUtils from './colorUtils';
import { getDateColor, getDateDescription } from './dateLogic';

function splitRectangle(rect: RectangleDto, ratio: number): [RectangleDto, RectangleDto] {
	let current: RectangleDto;
	let remaining: RectangleDto;

	if (rect.width > rect.height) {
		const splitWidth = rect.width * ratio;
		current = { x: rect.x, y: rect.y, width: splitWidth, height: rect.height };
		remaining = {
			x: rect.x + splitWidth,
			y: rect.y,
			width: rect.width - splitWidth,
			height: rect.height
		};
	} else {
		const splitHeight = rect.height * ratio;
		current = { x: rect.x, y: rect.y, width: rect.width, height: splitHeight };
		remaining = {
			x: rect.x,
			y: rect.y + splitHeight,
			width: rect.width,
			height: rect.height - splitHeight
		};
	}
	return [current, remaining];
}

function sliceAndDiceStocks(
	sector: TreemapSectorDto,
	sectorMarketCap: number,
	sectorRect: RectangleDto
) {
	const sortedStocks = [...sector.stocks].sort((a, b) => b.marketCap - a.marketCap);
	let remainingRect = { ...sectorRect };
	let remainingMarketCap = sectorMarketCap;

	for (const stock of sortedStocks) {
		const ratio = stock.marketCap / remainingMarketCap;
		remainingMarketCap -= stock.marketCap;
		const [stockRect, nextRemainingRect] = splitRectangle(remainingRect, ratio);
		stock.rectangle = stockRect;
		remainingRect = nextRemainingRect;
	}
}

export function calculateTreemapRectangles(
	data: TreemapDataDto,
	width: number,
	height: number
): TreemapDataDto {
	const sortedSectors = [...data.sectors].sort((a, b) => b.totalMarketCap - a.totalMarketCap);
	let remainingRect: RectangleDto = { x: 0, y: 0, width, height };
	let remainingMarketCap = data.totalMarketCap;

	const calculatedData = { ...data, sectors: sortedSectors };

	for (const sector of calculatedData.sectors) {
		const ratio = sector.totalMarketCap / remainingMarketCap;
		remainingMarketCap -= sector.totalMarketCap;
		const [sectorRect, nextRemainingRect] = splitRectangle(remainingRect, ratio);
		sector.rectangle = sectorRect;
		remainingRect = nextRemainingRect;
		sliceAndDiceStocks(sector, sector.totalMarketCap, sector.rectangle);
	}
	return calculatedData;
}

function currencyNameToSign(currencyName: string | undefined | null): string {
	switch (currencyName) {
		case 'USD':
			return '$';
		case 'TWD':
			return 'NT$';
		case 'GBP':
			return '£';
		default:
			return '';
	}
}

function formatPercent(value?: number): string {
	return value ? `${value.toFixed(2)}%` : '-';
}

function formatPrice(value?: number, currency?: string): string {
	return value ? `${currencyNameToSign(currency)}${value.toFixed(2)}` : 'N/A';
}

export function getCellDescription(stock: TreemapNodeDto, metric: MapMetric | null): string {
	if (metric === null) return '\n-';

	switch (metric) {
		case MapMetric.RegularMarketChangePercent:
			if (
				stock.regularMarketChangePercent === undefined ||
				stock.regularMarketChangePercent === null
			)
				return '\n-';
			return `\n${formatPercent(stock.regularMarketChangePercent)}\nPrice: ${formatPrice(stock.regularMarketPrice, stock.currency)}`;

		case MapMetric.PreMarketChangePercent:
			if (!stock.preMarketPrice) return '\n-';
			return `\n${formatPercent(stock.preMarketChangePercent)}\nPrice: ${formatPrice(stock.preMarketPrice, stock.currency)}`;

		case MapMetric.PostMarketChangePercent:
			if (!stock.postMarketPrice) return '\n-';
			return `\n${formatPercent(stock.postMarketChangePercent)}\nPrice: ${formatPrice(stock.postMarketPrice, stock.currency)}`;

		case MapMetric.MarketState:
			{ const state = stock.marketState?.trim().toUpperCase();
			switch (state) {
				case 'REGULAR':
					return '\nOPEN';
				case 'PRE':
					return '\nPRE-MARKET';
				case 'POST':
					return '\nPOST-MARKET';
				case 'CLOSED':
					return '\nCLOSED';
				default:
					return '\n-';
			} }

		case MapMetric.Volume:
			return stock.volume ? `\n${stock.volume.toLocaleString()}` : '\n-';

		case MapMetric.DividendYield:
			return stock.dividendYield ? `\n${(stock.dividendYield * 100).toFixed(2)}%` : '\n-';

		case MapMetric.DividendDate:
			return getDateDescription(stock.dividendDate, metric);
		case MapMetric.ExDividendDate:
			return getDateDescription(stock.exDividendDate, metric);
		case MapMetric.EarningsDate:
			return getDateDescription(stock.earningsDate, metric);

		case MapMetric.Beta:
			return stock.beta ? `\n${stock.beta.toFixed(2)}` : '\n-';
		case MapMetric.Pe:
			return stock.pe ? `\n${stock.pe.toFixed(2)}` : '\n-';
		case MapMetric.ForwardPe:
			return stock.forwardPe ? `\n${stock.forwardPe.toFixed(2)}` : '\n-';
		case MapMetric.ShortRatio:
			return stock.shortRatio ? `\n${stock.shortRatio.toFixed(2)}` : '\n-';
			
		case MapMetric.AnalystRecommendation:
			return stock.analystRecommendationKey
				? `\n${stock.analystRecommendationKey.split('_')
					.map(word => word.charAt(0).toUpperCase() + word.slice(1))
					.join(' ')}`
				: '\n-';
			
		case MapMetric.profitMargins:
			return stock.profitMargins ? `\n${formatPercent(stock.profitMargins * 100)}` : '\n-';
			
		case MapMetric.earningsQuarterlyGrowth:
			return stock.earningsQuarterlyGrowth ? `\n${formatPercent(stock.earningsQuarterlyGrowth * 100)}` : '\n-';
			
		case MapMetric.trailingEps:
			return stock.trailingEps ? `\n${stock.trailingEps.toFixed(2)}` : '\n-';

		default:
			return '\n-';
	}
}

const transparent = 'rgba(0,0,0,0)';
const red = { r: 255, g: 0, b: 0 };
const green = { r: 0, g: 255, b: 0 };

export function getCellColor(stock: TreemapNodeDto, metric: MapMetric | null): string {
	if (metric === null) return transparent;

	switch (metric) {
		case MapMetric.RegularMarketChangePercent:
			return stock.regularMarketChangePercent != null
				? ColorUtils.negativeDecreasingAlphaTransparentIncreasingAlphaPositive(
						stock.regularMarketChangePercent,
						-5,
						5,
						red,
						green
					)
				: transparent;

		case MapMetric.PreMarketChangePercent:
			return stock.preMarketChangePercent != null
				? ColorUtils.negativeDecreasingAlphaTransparentIncreasingAlphaPositive(
						stock.preMarketChangePercent,
						-5,
						5,
						red,
						green
					)
				: transparent;

		case MapMetric.PostMarketChangePercent:
			return stock.postMarketChangePercent != null
				? ColorUtils.negativeDecreasingAlphaTransparentIncreasingAlphaPositive(
						stock.postMarketChangePercent,
						-5,
						5,
						red,
						green
					)
				: transparent;

		case MapMetric.MarketState: {
			const state = stock.marketState?.trim().toUpperCase();
			switch (state) {
				case 'REGULAR':
					return 'rgba(41, 144, 59, 1)';
				case 'PRE':
					return 'rgba(41, 98, 255, 1)';
				case 'POST':
					return 'rgba(255, 152, 0, 1)';
				case 'CLOSED':
					return 'rgba(75, 75, 75, 1)';
				default:
					return transparent;
			}
		}

		case MapMetric.Volume:
			return stock.volume != null
				? ColorUtils.positiveIncreasingAlpha(stock.volume, 50_000_000, { r: 0, g: 178, b: 255 })
				: transparent;

		case MapMetric.DividendYield:
			return stock.dividendYield != null
				? ColorUtils.positiveIncreasingAlpha(stock.dividendYield * 100, 8.0, green)
				: transparent;

		case MapMetric.DividendDate:
			return getDateColor(stock.dividendDate);
		case MapMetric.ExDividendDate:
			return getDateColor(stock.exDividendDate);
		case MapMetric.EarningsDate:
			return getDateColor(stock.earningsDate);

		case MapMetric.Beta:
			return stock.beta != null
				? ColorUtils.threeWayScale(
						stock.beta,
						-1.0,
						1.0,
						2.5,
						{ r: 255, g: 81, b: 0 },
						{ r: 255, g: 0, b: 174 },
						{ r: 0, g: 255, b: 169 }
					)
				: transparent;

		case MapMetric.Pe:
			return stock.pe != null
				? ColorUtils.positiveDecreasingAlpha(stock.pe, 0.0, 40.0, green)
				: transparent;

		case MapMetric.ForwardPe:
			return stock.forwardPe != null
				? ColorUtils.positiveDecreasingAlpha(stock.forwardPe, 0.0, 30.0, green)
				: transparent;

		case MapMetric.ShortRatio:
			return stock.shortRatio != null
				? ColorUtils.positiveDecreasingAlpha(stock.shortRatio, 0.0, 12.0, green)
				: transparent;

		case MapMetric.AnalystRecommendation:
			return stock.analystRecommendationMean != null
				? ColorUtils.thresholdColor(
						stock.analystRecommendationMean,
						1.0,
						1.5,
						2.5,
						3.5,
						4.5,
						5.0,
						0.5
					)
				: transparent;

		case MapMetric.profitMargins:
			return stock.profitMargins != null
				? ColorUtils.negativeDecreasingAlphaTransparentIncreasingAlphaPositive(
						stock.profitMargins * 100,
						-50.0,
						50.0,
						red,
						green
					)
				: transparent;
			
		case MapMetric.earningsQuarterlyGrowth:
			return stock.earningsQuarterlyGrowth != null
				? ColorUtils.negativeDecreasingAlphaTransparentIncreasingAlphaPositive(
						stock.earningsQuarterlyGrowth * 100,
						-75.0,
						75.0,
						red,
						green
					)
				: transparent;
			
		case MapMetric.trailingEps:
			return stock.trailingEps != null
				? ColorUtils.negativeDecreasingAlphaTransparentIncreasingAlphaPositive(
						stock.trailingEps,
						-10.0,
						10.0,
						red,
						green
					)
				: transparent;

		default:
			return transparent;
	}
}
