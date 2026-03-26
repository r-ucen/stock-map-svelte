<script lang="ts">
	import StockTreeMap from '$lib/components/StockTreeMap.svelte';
	import { getContext } from 'svelte';
	import type { IState } from '$lib/Abstractions/IState';
	
	const s = getContext<IState>('state');
</script>

<StockTreeMap treemapData={s.treemapData} selectedMetric={s.selectedMetric}/>
