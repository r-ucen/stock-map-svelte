<script lang="ts">
	import { Badge } from "$lib/components/ui/badge/index.js";
	import { getContext, onMount } from 'svelte';
	import type { IState } from '$lib/Abstractions/IState';
	import * as Marker from "$lib/components/ui/marker/index.js";
	import RotateCwClock from "@lucide/svelte/icons/rotate-cw";

	let s = getContext<IState>("state");
	
	let now = $state(new Date());
	let lastUpdate = $derived(new Date(s.treemapData?.lastUpdated));
	
	onMount(() => {
		const interval = setInterval(() => {
			now = new Date();
		}, 1000);
		return () => clearInterval(interval);
	});
	
	function getFormattedDiff(startDate: Date, endDate: Date) {
		const diffSeconds = Math.floor((endDate.getTime() - startDate.getTime()) / 1000);
		
		const hours = Math.floor(diffSeconds / 3600);
		const minutes = Math.floor((diffSeconds % 3600) / 60);
		const seconds = diffSeconds % 60;

		return `${hours}h ${minutes}m ${seconds}s`;
	}
	
</script>

<div class="flex flex-col gap-1 px-3 py-2 text-xs">
	<Marker.Root variant="separator">
		<Marker.Icon>
			<RotateCwClock />
		</Marker.Icon>
		<Marker.Content>Updated {getFormattedDiff(lastUpdate, now)} ago</Marker.Content>
	</Marker.Root>
</div>
