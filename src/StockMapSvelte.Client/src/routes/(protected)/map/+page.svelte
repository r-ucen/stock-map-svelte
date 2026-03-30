<script lang="ts">
	import StockTreeMap from '$lib/components/StockTreeMap.svelte';
	import { getContext, onDestroy } from 'svelte';
	import type { IState } from '$lib/Abstractions/IState';
	import { fetchTreemapData } from '$lib/portfolioFetch';
	
	const s = getContext<IState>('state');

	let treemapInterval: ReturnType<typeof setInterval> | undefined;

	$effect(() => {
		clearInterval(treemapInterval);
		if (s.selectedPortfolioId) {
			fetchTreemapData(s);
			treemapInterval = setInterval(() => fetchTreemapData(s), 30000);
		}
	});

	onDestroy(() => {
		clearInterval(treemapInterval);
	});
</script>

<StockTreeMap treemapData={s.treemapData} selectedMetric={s.selectedMetric}/>
