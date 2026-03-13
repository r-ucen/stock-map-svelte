<script lang="ts">
	import DataTable from "./data-table.svelte";
	import { columns } from "./columns.js";
	import { getContext } from 'svelte';
	import type { IState } from '$lib/Abstractions/IState';

	let state = getContext<IState>('state');
	let data = $derived(state.portfolios)
</script>

<DataTable data={data} {columns}  />