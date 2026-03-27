<script lang="ts">
	import DataTable from './data-table.svelte';
	import { columns } from './columns.js';
	import { getContext } from 'svelte';
	import type { IAdminState } from '$lib/Abstractions/IAdminState';
	import type { IPortfolio } from '$lib/Abstractions/IState';

	let s = getContext<IAdminState>('stateAdmin');
	let { items }: { items: IPortfolio[] } = $props();

	$effect(() => {
		if (s.portfolios.length === 0 && items.length > 0) {
			s.portfolios = items;
		}
	});

</script>

<DataTable data={s.portfolios} {columns} />

