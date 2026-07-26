<script lang="ts">
	import DataTable from './data-table.svelte'
	import { columns } from "./columns.js";
	import { getContext } from 'svelte';
	import type { IAdminState } from '$lib/Abstractions/IAdminState';
	import type { IUser } from '$lib/Abstractions/IUser';
	import PaginationControls from '$lib/components/PaginationControls.svelte';
	import { page } from '$app/state';
	import { goto, invalidateAll } from '$app/navigation';

	let s = getContext<IAdminState>('stateAdmin');
	let { items, initialTotal }: {items: IUser[], initialTotal: number} = $props()

	let pageNumber = $state(Number(page.url.searchParams.get('PageNumber')) || 1);
	let pageSize = $state(Number(page.url.searchParams.get('PageSize')) || 10);

	let totalRecords = $derived(initialTotal || 0);

	let isFirstUrlSync = true;
	let isFirstRefresh = true;

	$effect(() => {
		s.users = items;
	});

	$effect(() => {
		const params = new URLSearchParams({
			PageNumber: pageNumber.toString(),
			PageSize: pageSize.toString()
		});

		const newSearch = `?${params.toString()}`;

		if (isFirstUrlSync) {
			isFirstUrlSync = false;
			return;
		}

		if (newSearch === page.url.search) { return; }

		// eslint-disable-next-line svelte/no-navigation-without-resolve
		goto(newSearch, { replaceState: true, keepFocus: true, noScroll: true });
	});

	$effect(() => {
		// eslint-disable-next-line @typescript-eslint/no-unused-expressions
		s.refreshUsers;
		if (isFirstRefresh) {
			isFirstRefresh = false;
			return;
		}
		invalidateAll();
	});

</script>

<DataTable data={s.users} {columns}  />

<PaginationControls
	count={totalRecords}
	perPage={pageSize}
	bind:page={pageNumber}
/>