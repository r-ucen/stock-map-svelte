<script lang="ts">
	import * as Select from "$lib/components/ui/select/index.js";
	import { MapMetric, type IState, getMetricLabel } from "$lib/Abstractions/IState";
	import { getContext } from "svelte";

	let s = getContext<IState>("state");

	const metrics = Object.values(MapMetric)
		.filter((v): v is MapMetric => typeof v === "number")
		.map((value) => ({
			label: getMetricLabel(value),
			value: value.toString()
		}));

	let selectedValue = $state(s.selectedMetric?.toString() ?? "");

	const selectedLabel = $derived(
		s.selectedMetric !== null ? getMetricLabel(s.selectedMetric) : "Select a metric"
	);

	function handleValueChange(v: string | undefined) {
		if (v !== undefined) {
			s.selectedMetric = Number(v) as MapMetric;
		}
	}
</script>

<Select.Root
	type="single"
	bind:value={selectedValue}
	onValueChange={handleValueChange}
>
	<Select.Trigger class="w-60">
		{selectedLabel}
	</Select.Trigger>
	<Select.Content>
		{#each metrics as metric (metric.value)}
			<Select.Item value={metric.value}>
				{metric.label}
			</Select.Item>
		{/each}
	</Select.Content>
</Select.Root>
