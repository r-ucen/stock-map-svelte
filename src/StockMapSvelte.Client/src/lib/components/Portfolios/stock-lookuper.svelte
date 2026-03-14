<script lang="ts">
	import Select from 'svelte-select';
	import { Button } from "$lib/components/ui/button/index.js";
	import { SvelteURLSearchParams } from 'svelte/reactivity';

	import { Badge } from "$lib/components/ui/badge/index.js";

	let { selectedStocks = $bindable() }: { selectedStocks: string[] } = $props();

	let selectedValue = $state<{ value: string, label: string } | null>(null);


	import { onMount } from 'svelte';
	import { apiFetch } from '$lib/apiFetch';
	let mounted = $state(false);
	onMount(() => mounted = true);


	function addSelectedStockToStocks() {
		if (selectedValue) {
			selectedStocks = [...selectedStocks, selectedValue.value];
			selectedValue = null;
		}
	}
	
	interface StockOption {
		id: string;
		tickerSymbol: string;
	}

	async function loadOptions(filterText: string) {
		if (filterText.length < 1) return [];

		const params = new SvelteURLSearchParams();
		params.append('filter', filterText);

		selectedStocks.forEach(ticker => {
			params.append('stocksInPortfolio', ticker);
		});

		const response = await apiFetch(`/stocks/possible-to-add?${params.toString()}`);

		if (!response.ok) return [];

		const data: StockOption[] = await response.json();

		return data.map((stock: StockOption) => ({
			value: stock.tickerSymbol,
			label: stock.tickerSymbol
		}));
	}
</script>

{#if mounted}
	<div>
		<Select
			{loadOptions}
			bind:value={selectedValue}
			placeholder="Search for a stock..."
		/>
	</div>
	
	<Button
		onclick={() => {addSelectedStockToStocks()}}
			>Add Ticker
	</Button>

	<div class="flex flex-wrap gap-2">
		{#each selectedStocks as ticker (ticker)}
			<Badge variant="secondary" class="pl-1 pr-2 py-0.5 flex items-center gap-1.5 h-7">
				<Button
					variant="ghost"
					size="icon"
					class="h-5 w-5 rounded-full"
					onclick={() => selectedStocks = selectedStocks.filter(s => s !== ticker)}
				>
					<span class="sr-only">Remove {ticker}</span>
					<svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M18 6 6 18"/><path d="m6 6 12 12"/></svg>
				</Button>
				<span class="text-xs font-semibold uppercase">{ticker}</span>
			</Badge>
		{/each}
	</div>
{/if}