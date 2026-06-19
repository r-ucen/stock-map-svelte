<script lang="ts">
	import StockTreeMap from '$lib/components/StockTreeMap.svelte';
	import { getContext, onDestroy } from 'svelte';
	import type { IState } from '$lib/Abstractions/IState';
	import { PUBLIC_API_BASE_URL } from '$env/static/public';
	
	const s = getContext<IState>('state');

	let eventSource: EventSource | undefined;
	
	function connectSSE(portfolioId: string){
		eventSource?.close();
		
		eventSource = new EventSource(
			`${PUBLIC_API_BASE_URL}/treemap-data/${portfolioId}/stream`,
			{ withCredentials: true }
		);
		
		eventSource.addEventListener('treemap-update', (event) => {
			s.treemapData = JSON.parse(event.data);
		});
		
		eventSource.onerror = () => {};
	}

	$effect(() => {
		if (s.selectedPortfolioId) {
			connectSSE(s.selectedPortfolioId);
		} else {
			eventSource?.close();
			eventSource = undefined;
		}
	});

	onDestroy(() => {
		eventSource?.close();
	});
</script>

<StockTreeMap treemapData={s.treemapData} selectedMetric={s.selectedMetric}/>
