<script lang="ts">
	import { MapMetric } from '$lib/Abstractions/IState';
	import { SvelteSet } from 'svelte/reactivity';
	import { type ContinuousMetricScale, getMetricScale, type MetricScale } from '$lib/services/treemap/metricScales';

	let { selectedMetric }: { selectedMetric: MapMetric | null } = $props();

	const scale = $derived(selectedMetric !== null ? getMetricScale(selectedMetric) : null);

	function buildGradient(s: ContinuousMetricScale, steps = 500): string {
		const stops: string[] = [];
		for (let i = 0; i <= steps; i++) {
			const v = s.min + ((s.max - s.min) * i) / steps;
			stops.push(`${s.colorAt(v)} ${(i / steps) * 100}%`);
		}
		return `linear-gradient(to right, ${stops.join(', ')})`;
	}

	function generateTicks(scale: MetricScale, min: number, max: number, count = 5): number[] {
		if (scale.type === 'continuous' && scale.explicitTicks !== null) {
			return scale.explicitTicks;
		}
		
		const set = new SvelteSet<number>();
		for (let i = 0; i < count; i++) {
			set.add(min + ((max - min) * i) / (count - 1));
		}
		if (min < 0 && max > 0) set.add(0);
		return Array.from(set).sort((a, b) => a - b);
	}

	const gradient = $derived(scale?.type === 'continuous' ? buildGradient(scale) : '');
	const ticks = $derived(
		scale?.type === 'continuous' ? generateTicks(scale, scale.min, scale.max) : []
	);
</script>

{#if selectedMetric !== null && scale}
	<div class="flex flex-col gap-1 px-3 py-2 text-xs">

		{#if scale.type === 'continuous'}
			<div class="h-4 w-full rounded" style="background-image: {gradient};"></div>
			<div class="flex justify-around">
				{#each ticks as tick (tick)}
					<span>
						{scale.formatTick(tick)}
					</span>
				{/each}
			</div>
		{:else}
			<div class="flex flex-wrap gap-3">
				{#each scale.bands as band (band.label)}
					<div class="flex items-center gap-1">
						<span class="h-3 w-3 rounded" style="background: {band.color};"></span>
						<span>{band.label}</span>
					</div>
				{/each}
			</div>
		{/if}
	</div>
{/if}