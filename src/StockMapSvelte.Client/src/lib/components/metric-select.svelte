<script lang="ts">
	import * as Select from "$lib/components/ui/select/index.js";
	import { MapMetric, getMetricLabel } from "$lib/Abstractions/IState";

	let { selectedMetric = $bindable<MapMetric | null>(null) } = $props();

	const metrics = Object.values(MapMetric)
		.filter((v): v is MapMetric => typeof v === "number")
		.map((value) => ({
			label: getMetricLabel(value),
			value: value.toString()
		}));

	function handleValueChange(v: string | undefined) {
		if (!v) {
			selectedMetric = null;
			return;
		}

		selectedMetric = Number(v) as MapMetric;
	}
</script>

<Select.Root
	type="single"
	value={selectedMetric?.toString() ?? ""}
	onValueChange={handleValueChange}
>
	<Select.Trigger class="w-60">
		{selectedMetric !== null ? getMetricLabel(selectedMetric) : "Select a metric"}
	</Select.Trigger>
	<Select.Content>
		{#each metrics as metric (metric.value)}
			<Select.Item value={metric.value}>
				{metric.label}
			</Select.Item>
		{/each}
	</Select.Content>
</Select.Root>
