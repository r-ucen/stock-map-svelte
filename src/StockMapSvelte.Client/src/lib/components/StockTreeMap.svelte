<script lang="ts">
	import { MapMetric } from '$lib/Abstractions/IState';
	import type { TreemapDataDto, TreemapSectorDto, TreemapNodeDto } from '$lib/Abstractions/Treemap';
	import { calculateTreemapRectangles, getCellColor, getCellDescription } from '$lib/services/treemap';
	import { mode } from 'mode-watcher';
	import { toast } from 'svelte-sonner'
	import { Spinner } from '$lib/components/ui/spinner';
	import { Button } from '$lib/components/ui/button';
	import * as Empty from "$lib/components/ui/empty/index.js";

	let { treemapData, selectedMetric }: {
			treemapData: TreemapDataDto | null,
			selectedMetric: MapMetric | null
	} = $props();

	const textColor = $derived(mode.current === 'light' ? 'black' : 'white');
	const strokeColor = $derived(mode.current === 'light' ? 'white' : 'black');

	let container: HTMLDivElement | undefined = $state();
	let width = $state(0);
	let height = $state(0);

	let calculatedTreemapData: TreemapDataDto = $derived.by(() => {
		if (treemapData && width > 0 && height > 0) {
			const dataCopy = JSON.parse(JSON.stringify(treemapData));
			return calculateTreemapRectangles(dataCopy, width, height);
		}
		return {
			sectors: [],
			totalMarketCap: 0
		};
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

	function handleStockClick(sector: TreemapSectorDto, stock: TreemapNodeDto, info: string) {
		toast(`${stock.tickerSymbol}${stock.fullName ? ` : ${stock.fullName}` : ''}`, {
			description: `Sector: ${sector.sectorName}\n${info}`,
			descriptionClass: "whitespace-pre-line",
		});
	}
</script>

<div
	bind:this={container}
	class="absolute inset-0 overflow-hidden"
>
	{#if treemapData}
		{#if calculatedTreemapData.sectors.length > 0 && width > 100}
		<svg {width} {height} viewBox="0 0 {width} {height}" preserveAspectRatio="none" style="display: block;">
			{#each calculatedTreemapData.sectors as sector (sector.sectorName)}
				{#each sector.stocks as stock (stock.tickerSymbol)}
					{@const info = getCellDescription(stock, selectedMetric)}
					{@const color = getCellColor(stock, selectedMetric)}
					{@const initialFontSize = Math.max(12, Math.floor(stock.rectangle.width * 0.008))}
					{@const fontSize = (stock.rectangle.width < 50 || stock.rectangle.height < 30) ? Math.max(6, Math.floor(initialFontSize * 0.75)) : initialFontSize}
					{@const charWidth = fontSize * 0.6}
					{@const lineHeight = fontSize * 1.2}
					{@const textContent = stock.tickerSymbol + info}
					{@const maxLineLen = Math.max(stock.tickerSymbol.length, info.length)}
					{@const newlineCount = (textContent.match(/\n/g) || []).length}
					{@const showFull = maxLineLen * charWidth <= stock.rectangle.width && (lineHeight * newlineCount) <= stock.rectangle.height}
					{@const showTicker = !showFull && stock.tickerSymbol.length * charWidth <= stock.rectangle.width && lineHeight <= stock.rectangle.height}
					{@const lines = textContent.split('\n')}
					{@const startDy = -((lines.length - 1) * 0.6)}

					<g
						onclick={() => handleStockClick(sector, stock, info)}
						role="button"
						tabindex="0"
						style="outline: none; cursor: pointer;"
						onkeydown={(e) => {
							if (e.key === 'Enter' || e.key === ' ') {
								handleStockClick(sector, stock, info);
							}
						}}
					>
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
		{:else }
			<div class="flex items-center justify-center h-full">
				<Empty.Root>
					<Empty.Header>
						<Empty.Title>No stocks</Empty.Title>
						<Empty.Description>No stocks in this portfolio. To add some, edit the portfolio via clicking the three dots next to the portfolio.</Empty.Description>
					</Empty.Header>
					<Empty.Content>
						<Button variant="link" href="/portfolios">Go to portfolio management</Button>
					</Empty.Content>
				</Empty.Root>
			</div>
		{/if}
	{:else }
		<div class="flex items-center justify-center h-full">
			<Spinner />
		</div>
	{/if}
</div>