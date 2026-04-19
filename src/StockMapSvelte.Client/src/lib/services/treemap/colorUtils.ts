type Color = { r: number; g: number; b: number };

export function toRgbaString(color: Color, alpha: number): string {
	const clampedAlpha = Math.max(0, Math.min(1, alpha));
	return `rgba(${color.r}, ${color.g}, ${color.b}, ${clampedAlpha.toFixed(2)})`;
}

function clamp(value: number, min: number, max: number): number {
	return Math.max(min, Math.min(value, max));
}

export function threeWayScale(
	value: number,
	bottom: number,
	middle: number,
	top: number,
	bottomColor: Color,
	middleColor: Color,
	topColor: Color
): string {
	const clamped = clamp(value, bottom, top);
	if (clamped < 0) {
		const ratio = clamped / bottom;
		return toRgbaString(bottomColor, ratio);
	}
	if (clamped > 0) {
		if (clamped < middle) {
			const ratio = 1 - clamped / middle;
			return toRgbaString(middleColor, ratio);
		} else {
			const ratio = (clamped - middle) / (top - middle);
			return toRgbaString(topColor, ratio);
		}
	}
	return toRgbaString({ r: 0, g: 0, b: 0 }, 0);
}

export function negativeIncreasingAlphaTransparentDecreasingAlphaPositive(
	value: number,
	lo: number,
	hi: number,
	negative: Color,
	positive: Color
): string {
	const clamped = clamp(value, lo, hi);
	if (clamped < 0) {
		const ratio = 1 - clamped / lo;
		return toRgbaString(negative, ratio);
	}
	if (clamped > 0) {
		const ratio = 1 - clamped / hi;
		return toRgbaString(positive, ratio);
	}
	return toRgbaString({ r: 0, g: 0, b: 0 }, 0);
}

export function negativeDecreasingAlphaTransparentIncreasingAlphaPositive(
	value: number,
	lo: number,
	hi: number,
	negative: Color,
	positive: Color
): string {
	if (lo >= 0 || hi <= 0) {
		return toRgbaString({ r: 0, g: 0, b: 0 }, 0);
	}
	const clamped = clamp(value, lo, hi);
	if (clamped < 0) {
		const ratio = clamped / lo;
		return toRgbaString(negative, ratio);
	}
	if (clamped > 0) {
		const ratio = clamped / hi;
		return toRgbaString(positive, ratio);
	}
	return toRgbaString({ r: 0, g: 0, b: 0 }, 0);
}

export function positiveIncreasingAlpha(value: number, hi: number, color: Color): string {
	if (value < 0) return toRgbaString({ r: 0, g: 0, b: 0 }, 0);
	const clamped = clamp(value, 0, hi);
	const ratio = clamped / hi;
	return toRgbaString(color, ratio);
}

export function positiveDecreasingAlpha(
	value: number,
	lo: number,
	hi: number,
	color: Color
): string {
	if (value < 0) return toRgbaString({ r: 0, g: 0, b: 0 }, 0);
	const clamped = clamp(value, lo, hi);
	const ratio = 1 - clamped / hi;
	return toRgbaString(color, ratio);
}

export function thresholdColor(
	value: number,
	lo: number,
	strong_buy: number,
	buy: number,
	threshold_hold: number,
	sell: number,
	hi: number,
	alpha: number
): string {
	const clamped = clamp(value, lo, hi);

	if (clamped <= strong_buy) {
		return toRgbaString({r: 0, g: 255, b: 0 }, alpha);
	} else if (clamped <= buy) {
		return toRgbaString({ r: 175, g: 255, b: 0 }, alpha);
	} else if (clamped <= threshold_hold) {
		return toRgbaString({ r: 255, g: 255, b: 0 }, alpha);
	} else if (clamped <= sell) {
		return toRgbaString({ r: 255, g: 175, b: 0 }, alpha);
	} else {
		return toRgbaString({ r: 255, g: 0, b: 0 }, alpha);
	}
}
