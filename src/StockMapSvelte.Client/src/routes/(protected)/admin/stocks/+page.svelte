<script lang="ts">
	import type { IStock } from '$lib/Abstractions/IStock';
	import Stocks from '$lib/components/Stocks/Stocks.svelte';

	let { data }: {
		data: {
			stocks: IStock[];
		};
	} = $props();
</script>

<Stocks items={data.stocks} />
