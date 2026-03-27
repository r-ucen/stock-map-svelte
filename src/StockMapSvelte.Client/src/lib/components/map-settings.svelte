<script lang="ts">
	import MetricSelect from "$lib/components/metric-select.svelte";
	import type { IState } from '$lib/Abstractions/IState';
	import { getContext } from 'svelte';

	let s = getContext<IState>("state");
</script>

<div class="flex w-full items-center justify-center p-2">
	<MetricSelect bind:selectedMetric={s.selectedMetric} />
</div>
