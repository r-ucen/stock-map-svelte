<script lang="ts">
	import DataTable from '$lib/components/StockProfiles/data-table.svelte';
	import { columns } from './columns.js';
	import { getContext } from 'svelte';
	import type { IAdminState } from '$lib/Abstractions/IAdminState';
	import type { IStockProfile } from '$lib/Abstractions/IStockProfile';
	import PaginationControls from '$lib/components/PaginationControls.svelte';
	import { apiFetch } from '$lib/apiFetch';
	import { toast } from 'svelte-sonner';

	let s = getContext<IAdminState>('stateAdmin');
	let { items, initialTotal  }: { items: IStockProfile[], initialTotal: number } = $props();

	$effect(() => {
		if (s.stockProfiles.length === 0 && items.length > 0) {
			s.stockProfiles = items;
		}
	});

	let pageNumber = $state(1);
	let pageSize = $state(10);
	let totalRecords = $derived(initialTotal || 0);

	$effect(() => {
		async function fetchPagedStocks() {
			try {
				const params = new URLSearchParams({
					PageNumber: pageNumber.toString(),
					PageSize: pageSize.toString()
				});

				const res = await apiFetch(`/stock-profiles?${params.toString()}`);

				if (res.ok) {
					const result = await res.json();
					s.stockProfiles = result.data;
					totalRecords = result.totalRecords;
				} else {
					toast.error("Failed to fetch stocks");
				}
			} catch (error) {
				console.error("Error fetching stocks:", error);
			}
		}

		fetchPagedStocks();
	});

</script>

<DataTable data={s.stockProfiles} {columns} />

<PaginationControls
	count={totalRecords}
	perPage={pageSize}
	bind:page={pageNumber}
/>