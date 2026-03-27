<script lang="ts">
	import Portfolios from '$lib/components/Portfolios/Portfolios.svelte';
	import type { IPortfolio } from '$lib/Abstractions/IState';

	let { data }: {
		data: {
			portfolios: IPortfolio[];
		};
	} = $props();
	
</script>

<Portfolios items={data.portfolios} />