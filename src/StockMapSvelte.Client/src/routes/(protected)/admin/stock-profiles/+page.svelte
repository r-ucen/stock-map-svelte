<script lang="ts">
	import type { IStockProfile } from '$lib/Abstractions/IStockProfile';
	import StockProfiles from '$lib/components/StockProfiles/StockProfiles.svelte';

	let { data }: {
		data: {
			stockProfiles: IStockProfile[];
		};
	} = $props();
</script>

<StockProfiles items={data.stockProfiles} />

