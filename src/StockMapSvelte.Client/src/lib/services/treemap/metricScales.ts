import { MapMetric } from '$lib/Abstractions/IState';
import * as ColorUtils from './colorUtils';
import { negativeIncreasingAlphaTransparentDecreasingAlphaPositive } from './colorUtils';

type Color = { r: number; g: number; b: number };
const red: Color = { r: 255, g: 0, b: 0 };
const green: Color = { r: 0, g: 255, b: 0 };
const blue: Color = { r: 0, g: 178, b: 255 };

export interface ContinuousMetricScale {
	type: 'continuous';
	min: number;
	max: number;
	colorAt: (value: number) => string;
	formatTick: (value: number) => string;
	explicitTicks: number[] | null;
}

export interface DiscreteMetricScale {
	type: 'discrete';
	bands: { label: string; color: string }[];
}

export type MetricScale = ContinuousMetricScale | DiscreteMetricScale;

const pct = (v: number) => `${v.toFixed(1)}%`;
const num = (v: number) => v.toFixed(1);
const compact = (v: number) => new Intl.NumberFormat('en-US', { notation: 'compact' }).format(v);
const days = (v: number) => `${Math.round(v)}d`;

export function getMetricScale(metric: MapMetric): MetricScale | null {
	switch (metric) {
		case MapMetric.RegularMarketChangePercent:
		case MapMetric.PreMarketChangePercent:
		case MapMetric.PostMarketChangePercent:
			return {
				type: 'continuous',
				min: -5,
				max: 5,
				colorAt: (v) =>
					ColorUtils.negativeDecreasingAlphaTransparentIncreasingAlphaPositive(
						v,
						-5,
						5,
						red,
						green
					),
				formatTick: pct,
				explicitTicks: null
			};

		case MapMetric.Volume:
			return {
				type: 'continuous',
				min: 0,
				max: 50_000_000,
				colorAt: (v) => ColorUtils.positiveIncreasingAlpha(v, 50_000_000, blue),
				formatTick: compact,
				explicitTicks: null
			};

		case MapMetric.DividendYield:
			return {
				type: 'continuous',
				min: 0,
				max: 8,
				colorAt: (v) => ColorUtils.positiveIncreasingAlpha(v, 8.0, green),
				formatTick: pct,
				explicitTicks: null
			};

		case MapMetric.Beta:
			return {
				type: 'continuous',
				min: -1,
				max: 2.5,
				colorAt: (v) =>
					ColorUtils.threeWayScale(
						v,
						-1.0,
						1.0,
						2.5,
						{ r: 255, g: 81, b: 0 },
						{ r: 255, g: 0, b: 174 },
						{ r: 0, g: 255, b: 169 }
					),
				formatTick: num,
				explicitTicks: [-1, 0, 1, 1.5, 2.5]
			};

		case MapMetric.Pe:
			return {
				type: 'continuous',
				min: 0,
				max: 40,
				colorAt: (v) => ColorUtils.positiveDecreasingAlpha(v, 0, 40, green),
				formatTick: num,
				explicitTicks: null
			};

		case MapMetric.ForwardPe:
			return {
				type: 'continuous',
				min: 0,
				max: 30,
				colorAt: (v) => ColorUtils.positiveDecreasingAlpha(v, 0, 30, green),
				formatTick: num,
				explicitTicks: null
			};

		case MapMetric.ShortRatio:
			return {
				type: 'continuous',
				min: 0,
				max: 12,
				colorAt: (v) => ColorUtils.positiveDecreasingAlpha(v, 0, 12, green),
				formatTick: num,
				explicitTicks: null
			};

		case MapMetric.profitMargins:
			return {
				type: 'continuous',
				min: -50,
				max: 50,
				colorAt: (v) =>
					ColorUtils.negativeDecreasingAlphaTransparentIncreasingAlphaPositive(
						v,
						-50,
						50,
						red,
						green
					),
				formatTick: pct,
				explicitTicks: null
			};

		case MapMetric.earningsQuarterlyGrowth:
			return {
				type: 'continuous',
				min: -75,
				max: 75,
				colorAt: (v) =>
					ColorUtils.negativeDecreasingAlphaTransparentIncreasingAlphaPositive(
						v,
						-75,
						75,
						red,
						green
					),
				formatTick: pct,
				explicitTicks: null
			};

		case MapMetric.trailingEps:
			return {
				type: 'continuous',
				min: -20,
				max: 20,
				colorAt: (v) =>
					ColorUtils.negativeDecreasingAlphaTransparentIncreasingAlphaPositive(
						v,
						-20,
						20,
						red,
						green
					),
				formatTick: num,
				explicitTicks: null
			};

		case MapMetric.forwardEps:
			return {
				type: 'continuous',
				min: -30,
				max: 30,
				colorAt: (v) =>
					ColorUtils.negativeDecreasingAlphaTransparentIncreasingAlphaPositive(
						v,
						-30,
						30,
						red,
						green
					),
				formatTick: num,
				explicitTicks: null
			};

		case MapMetric.pegRatio:
			return {
				type: 'continuous',
				min: 0,
				max: 2,
				colorAt: (v) => ColorUtils.positiveDecreasingAlpha(v, 0, 2, green),
				formatTick: num,
				explicitTicks: null
			};

		case MapMetric.oneYearChange:
			return {
				type: 'continuous',
				min: -100,
				max: 200,
				colorAt: (v) =>
					ColorUtils.negativeDecreasingAlphaTransparentIncreasingAlphaPositive(
						v,
						-100,
						200,
						red,
						green
					),
				formatTick: pct,
				explicitTicks: [-100, 0, 150, 200]
			};

		case MapMetric.currentPriceToMedianTargetPriceChange:
			return {
				type: 'continuous',
				min: -100,
				max: 100,
				colorAt: (v) =>
					ColorUtils.negativeDecreasingAlphaTransparentIncreasingAlphaPositive(
						v,
						-100,
						100,
						red,
						green
					),
				formatTick: pct,
				explicitTicks: null
			};

		case MapMetric.totalDebt:
			return {
				type: 'continuous',
				min: 0,
				max: 100_000_000_000,
				colorAt: (v) => ColorUtils.positiveIncreasingAlpha(v, 100_000_000_000, green),
				formatTick: compact,
				explicitTicks: null
			};

		case MapMetric.freeCashflow:
			return {
				type: 'continuous',
				min: 0,
				max: 50_000_000_000,
				colorAt: (v) => ColorUtils.positiveIncreasingAlpha(v, 50_000_000_000, green),
				formatTick: compact,
				explicitTicks: null
			};

		case MapMetric.revenueGrowth:
			return {
				type: 'continuous',
				min: -100,
				max: 100,
				colorAt: (v) =>
					ColorUtils.negativeDecreasingAlphaTransparentIncreasingAlphaPositive(
						v,
						-100,
						100,
						red,
						green
					),
				formatTick: pct,
				explicitTicks: null
			};

		case MapMetric.MarketState:
			return {
				type: 'discrete',
				bands: [
					{ label: 'Open', color: 'rgba(41, 144, 59, 1)' },
					{ label: 'Pre-Market', color: 'rgba(41, 98, 255, 1)' },
					{ label: 'Post-Market', color: 'rgba(255, 152, 0, 1)' },
					{ label: 'Closed', color: 'rgba(75, 75, 75, 1)' }
				]
			};

		case MapMetric.AnalystRecommendation:
			return {
				type: 'discrete',
				bands: [
					{ label: 'Strong Buy', color: ColorUtils.toRgbaString({ r: 0, g: 255, b: 0 }, 0.5) },
					{ label: 'Buy', color: ColorUtils.toRgbaString({ r: 175, g: 255, b: 0 }, 0.5) },
					{ label: 'Hold', color: ColorUtils.toRgbaString({ r: 255, g: 255, b: 0 }, 0.5) },
					{ label: 'Sell', color: ColorUtils.toRgbaString({ r: 255, g: 175, b: 0 }, 0.5) },
					{ label: 'Strong Sell', color: ColorUtils.toRgbaString({ r: 255, g: 0, b: 0 }, 0.5) }
				]
			};

		case MapMetric.DividendDate:
		case MapMetric.ExDividendDate:
		case MapMetric.EarningsDate:
			return {
				type: 'continuous',
				min: -90,
				max: 90,
				colorAt: (v) =>
					negativeIncreasingAlphaTransparentDecreasingAlphaPositive(
						v,
						-90,
						90,
						{ r: 255, g: 0, b: 0 },
						{ r: 0, g: 128, b: 0 }
					),
				formatTick: days,
				explicitTicks: null
			};

		default:
			return null;
	}
}
