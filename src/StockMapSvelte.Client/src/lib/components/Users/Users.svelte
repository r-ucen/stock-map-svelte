<script lang="ts">
	import DataTable from './data-table.svelte'
	import { columns } from "./columns.js";
	import { getContext } from 'svelte';
	import type { IAdminState } from '$lib/Abstractions/IAdminState';
	import type { IUser } from '$lib/Abstractions/IUser';

	let s = getContext<IAdminState>('stateAdmin');
	let { items }: {items: IUser[]} = $props();

	$effect(() => {
		if (s.users.length === 0 && items.length > 0) {
			s.users = items;
		}
	});

</script>

<DataTable data={s.users} {columns}  />
