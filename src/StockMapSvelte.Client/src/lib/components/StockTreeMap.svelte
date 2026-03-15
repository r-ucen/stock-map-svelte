<script lang="ts">
	import { getContext } from 'svelte';
	import type { IState } from '$lib/Abstractions/IState';
	import type { TreemapDataDto } from '$lib/Abstractions/Treemap';
	import { calculateTreemapRectangles, getCellColor, getCellDescription } from '$lib/services/treemap';
	import { mode } from 'mode-watcher';

	const s: IState = getContext<IState>('state');

	const textColor = $derived(mode.current === 'light' ? 'black' : 'white');
	const strokeColor = $derived(mode.current === 'light' ? 'white' : 'black');

	let container: HTMLDivElement | undefined = $state();
	let width = $state(0);
	let height = $state(0);

	let calculatedTreemapData: TreemapDataDto | null = $derived.by(() => {
		if (s.treemapData && width > 0 && height > 0) {
			const dataCopy = JSON.parse(JSON.stringify(s.treemapData));
			return calculateTreemapRectangles(dataCopy, width, height);
		}
		return null;
	});

	$effect(() => {
		if (!container) return;

		const resizeObserver = new ResizeObserver((entries) => {
			for (const entry of entries) {
				width = entry.contentRect.width;
				height = entry.contentRect.height;
			}
		});

		resizeObserver.observe(container);

		return () => {
			resizeObserver.disconnect();
		};
	});
</script>

<div
	bind:this={container}
	class="absolute inset-0 overflow-hidden"
>
	{#if calculatedTreemapData?.sectors && width > 100}
		<svg {width} {height} viewBox="0 0 {width} {height}" preserveAspectRatio="none" style="display: block;">
			{#each calculatedTreemapData.sectors as sector (sector.sectorName)}
				{#each sector.stocks as stock (stock.tickerSymbol)}
					{@const info = getCellDescription(stock, s.selectedMetric)}
					{@const color = getCellColor(stock, s.selectedMetric)}
					{@const fontSize = Math.max(12, Math.floor(stock.rectangle.width * 0.008))}
					{@const charWidth = fontSize * 0.6}
					{@const lineHeight = fontSize * 1.2}
					{@const showTicker = stock.tickerSymbol.length * charWidth <= stock.rectangle.width && lineHeight <= stock.rectangle.height}
					{@const showFull = (stock.tickerSymbol.length * charWidth <= stock.rectangle.width) && (lineHeight * (info.split('\n').length + 1) <= stock.rectangle.height) && (stock.rectangle.width > 50 && stock.rectangle.height > 30)}
					{@const lines = (stock.tickerSymbol + info).split('\n')}
					{@const startDy = -((lines.length - 1) * 0.6)}

					<g>
						<rect
							x={stock.rectangle.x}
							y={stock.rectangle.y}
							width={stock.rectangle.width}
							height={stock.rectangle.height}
							fill={color}
							stroke={strokeColor}
							stroke-width="1"
							rx="5"
							ry="5"
						>
							<title>{stock.tickerSymbol} {info.replace(/\n/g, ' ')}</title>
						</rect>

						<text
							x={stock.rectangle.x + stock.rectangle.width / 2}
							y={stock.rectangle.y + stock.rectangle.height / 2}
							text-anchor="middle"
							dominant-baseline="middle"
							fill={textColor}
							font-size="{fontSize}px"
							style="pointer-events: none;"
						>
							{#if showFull}
								{#each lines as line, i (i)}
									<tspan x={stock.rectangle.x + stock.rectangle.width / 2} dy={i === 0 ? `${startDy}em` : "1.2em"}>
										{line}
									</tspan>
								{/each}
							{:else if showTicker}
								{stock.tickerSymbol}
							{/if}
						</text>
					</g>
				{/each}
			{/each}
		</svg>
	{/if}
</div>