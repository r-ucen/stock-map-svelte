<script lang="ts">
	import DataTable from '$lib/components/StockProfiles/data-table.svelte';
	import { columns } from './columns.js';
	import { getContext } from 'svelte';
	import type { IAdminState } from '$lib/Abstractions/IAdminState';
	import type { IStockProfile } from '$lib/Abstractions/IStockProfile';

	let s = getContext<IAdminState>('stateAdmin');
	let { items }: { items: IStockProfile[] } = $props();

	$effect(() => {
		if (s.stockProfiles.length === 0 && items.length > 0) {
			s.stockProfiles = items;
		}
	});

</script>

<DataTable data={s.stockProfiles} {columns} />

