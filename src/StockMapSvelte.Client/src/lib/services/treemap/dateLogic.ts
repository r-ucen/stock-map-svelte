import { MapMetric } from '$lib/Abstractions/IState';
import { negativeIncreasingAlphaTransparentDecreasingAlphaPositive } from './colorUtils';

export function getDateDescription(dateStr: string | undefined | null, metric: MapMetric): string {
	if (!dateStr) return '\n-';

	const date = new Date(dateStr);
	if (isNaN(date.getTime())) return '\n-';

	const now = new Date();
	const diffMilliseconds = date.getTime() - now.getTime();
	const diffDays = diffMilliseconds / (1000 * 60 * 60 * 24);
	const diffTotalMinutes = diffMilliseconds / (1000 * 60);

	if (diffDays < -90 || diffDays > 360) {
		return '\n-';
	}

	const dateString = new Intl.DateTimeFormat(undefined, {
		day: 'numeric',
		month: 'numeric',
		year: 'numeric',
		weekday: 'long'
	}).format(date);

	if (date.toDateString() === now.toDateString()) {
		if (metric !== MapMetric.EarningsDate) return `\n${dateString}\nToday`;

		let timeString = '';
		if (diffTotalMinutes > 0) {
			if (diffTotalMinutes < 60) {
				timeString = `in ${Math.round(diffTotalMinutes)} minutes`;
			} else {
				const hours = Math.floor(diffTotalMinutes / 60);
				const minutes = Math.round(diffTotalMinutes % 60);
				timeString = `in ${hours} hours${minutes > 0 ? ` and ${minutes} minutes` : ''}`;
			}
		} else {
			const absDiffMinutes = Math.abs(diffTotalMinutes);
			if (absDiffMinutes < 60) {
				timeString = `${Math.round(absDiffMinutes)} minutes ago`;
			} else {
				const hours = Math.floor(absDiffMinutes / 60);
				const minutes = Math.round(absDiffMinutes % 60);
				timeString = `${hours} hours${minutes > 0 ? ` and ${minutes} minutes` : ''} ago`;
			}
		}
		return `\n${dateString}\nToday\n${timeString}`;
	}

	if (diffDays > 0) {
		return `\n${dateString}\nin ${Math.ceil(diffDays)} days`;
	}

	return `\n${dateString}\n${Math.floor(-diffDays)} days ago`;
}

export function getDateColor(dateStr: string | undefined | null): string {
	if (!dateStr) return 'rgba(0,0,0,0)';

	const date = new Date(dateStr);
	if (isNaN(date.getTime())) return 'rgba(0,0,0,0)';

	const now = new Date();
	const differenceInDays = (date.getTime() - now.getTime()) / (1000 * 60 * 60 * 24);

	if (differenceInDays < -90 || differenceInDays > 360) {
		return 'rgba(0,0,0,0)';
	}

	const hiDividendDays = 90;
	const loDividendDays = -90;

	return negativeIncreasingAlphaTransparentDecreasingAlphaPositive(
		differenceInDays,
		loDividendDays,
		hiDividendDays,
		{ r: 255, g: 0, b: 0 },
		{ r: 0, g: 128, b: 0 }
	);
}
