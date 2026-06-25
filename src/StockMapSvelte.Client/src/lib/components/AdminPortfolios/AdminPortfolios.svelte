<script lang="ts">
	import DataTable from './data-table.svelte';
	import { columns } from './columns.js';
	import { getContext } from 'svelte';
	import type { IAdminState } from '$lib/Abstractions/IAdminState';
	import type { IPortfolio } from '$lib/Abstractions/IState';
	import { apiFetch } from '$lib/apiFetch';
	import { toast } from 'svelte-sonner';
	import PaginationControls from '$lib/components/PaginationControls.svelte';

	let s = getContext<IAdminState>('stateAdmin');
	let { items, initialTotal }: { items: IPortfolio[], initialTotal: number } = $props();

	$effect(() => {
		if (s.portfolios.length === 0 && items?.length > 0) {
			s.portfolios = items;
		}
	});

	let pageNumber = $state(1);
	let pageSize = $state(10);
	let totalRecords = $derived(initialTotal || 0);

	$effect(() => {
		async function fetchPagedPortfolios() {
			try {
				const params = new URLSearchParams({
					PageNumber: pageNumber.toString(),
					PageSize: pageSize.toString()
				});

				const res = await apiFetch(`/portfolios?${params.toString()}`);

				if (res.ok) {
					const result = await res.json();
					s.portfolios = result.data;
					totalRecords = result.totalRecords;
				} else {
					toast.error("Failed to fetch portfolios");
				}
			} catch (error) {
				console.error("Error fetching portfolios:", error);
			}
		}

		fetchPagedPortfolios();
	});

</script>

<DataTable data={s.portfolios} {columns} />

<PaginationControls
	count={totalRecords}
	perPage={pageSize}
	bind:page={pageNumber}
/>
