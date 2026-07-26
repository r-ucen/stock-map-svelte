<script lang="ts">
	import DataTable from '$lib/components/StockProfiles/data-table.svelte';
	import { columns } from './columns.js';
	import { getContext } from 'svelte';
	import type { IAdminState } from '$lib/Abstractions/IAdminState';
	import type { IStockProfile } from '$lib/Abstractions/IStockProfile';
	import PaginationControls from '$lib/components/PaginationControls.svelte';
	import { page } from '$app/state';
	import { goto } from '$app/navigation';

	let s = getContext<IAdminState>('stateAdmin');
	let { items, initialTotal  }: { items: IStockProfile[], initialTotal: number } = $props();

	let pageNumber = $state(Number(page.url.searchParams.get('PageNumber')) || 1);
	let pageSize = $state(Number(page.url.searchParams.get('PageSize')) || 10);

	let totalRecords = $derived(initialTotal || 0);

	let isFirstUrlSync = true;
	
	$effect(() => {
		s.stockProfiles = items;
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

</script>

<DataTable data={s.stockProfiles} {columns} />

<PaginationControls
	count={totalRecords}
	perPage={pageSize}
	bind:page={pageNumber}
/>