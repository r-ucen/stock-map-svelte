<script lang="ts">
	import type { IPortfolio } from '$lib/Abstractions/IState';
	import AdminPortfolios from '$lib/components/AdminPortfolios/AdminPortfolios.svelte';

	let { data }: {
		data: {
			portfolios: IPortfolio[];
			initialTotal: number;
		};
	} = $props();
</script>

<AdminPortfolios items={data.portfolios} initialTotal={data.initialTotal} />

