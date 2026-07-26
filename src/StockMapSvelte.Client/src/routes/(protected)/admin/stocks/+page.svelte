<script lang="ts">
	import type { IStock } from '$lib/Abstractions/IStock';
	import Stocks from '$lib/components/Stocks/Stocks.svelte';

	let { data }: {
		data: {
			items: IStock[];
			initialTotal: number;
		};
	} = $props();
</script>

<Stocks items={data.items} initialTotal={data.initialTotal} />
