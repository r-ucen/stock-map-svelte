<script lang="ts">
	import DataTable from '$lib/components/Stocks/data-table.svelte'
	import { columns } from "./columns.js";
	import { getContext } from 'svelte';
	import { Button, buttonVariants } from '$lib/components/ui/button';
	import Plus  from "@lucide/svelte/icons/plus";
	import { Input } from '$lib/components/ui/input';
	import { Label } from '$lib/components/ui/label';
	import * as Dialog from "$lib/components/ui/dialog/index.js";
	import { toast } from 'svelte-sonner';
	import type { IAdminState } from '$lib/Abstractions/IAdminState';
	import type { IStock } from '$lib/Abstractions/IStock';
	import { createStock } from '$lib/components/Stocks/stockActions';
	import { apiFetch } from '$lib/apiFetch';
	import PaginationControls from '$lib/components/PaginationControls.svelte';

	let s = getContext<IAdminState>('stateAdmin');
	let { items, initialTotal }: {items: IStock[], initialTotal: number} = $props();

	$effect(() => {
		if (s.stocks.length === 0 && items?.length > 0) {
			s.stocks = items;
		}
	});
	
	let pageNumber = $state(1);
	let pageSize = $state(10);
	let totalRecords = $derived(initialTotal || 0);

	let open = $state(false);

	let isBeingProcessed = $state(false);

	let stockTickerBeingCreated = $state("");

	$effect(() => {
		async function fetchPagedStocks() {
			try {
				const params = new URLSearchParams({
					PageNumber: pageNumber.toString(),
					PageSize: pageSize.toString()
				});

				const res = await apiFetch(`/stocks?${params.toString()}`);

				if (res.ok) {
					const result = await res.json();
					s.stocks = result.data;
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

	function resetCreateForm() {
		stockTickerBeingCreated = "";
	}

	async function handleCreateSubmit(e: SubmitEvent){
		e.preventDefault();
		isBeingProcessed = true;
		const result = await createStock(s, stockTickerBeingCreated);
		if (!result.success) {
			toast.error(result.error ?? "An error occurred while creating new stock");
		} else {
			resetCreateForm();
			open = false;
			toast.success("Successfully created stock");
			pageNumber = 1;
		}
		isBeingProcessed = false;
	}
</script>

<Button
	variant="outline"
	size="sm"
	onclick={() => open = true}
>
	<Plus /> Create Stock
</Button>

<Dialog.Root bind:open>
	<Dialog.Content class="sm:max-w-[425px]" onInteractOutside={() => resetCreateForm()}>
		<form onsubmit={handleCreateSubmit}>
			<Dialog.Header>
				<Dialog.Title>Create New Stock</Dialog.Title>
			</Dialog.Header>
			<div class="grid gap-4 py-4">
				<div class="grid gap-2">
					<Label for="name">Ticker Symbol</Label>
					<Input
						id="name"
						required
						bind:value={stockTickerBeingCreated}
						disabled={isBeingProcessed}
					/>
				</div>
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: "outline" })} onclick={resetCreateForm}>
					Cancel
				</Dialog.Close>
				<Button
					type="submit"
					disabled={isBeingProcessed}
				>Submit</Button>
			</Dialog.Footer>
		</form>
	</Dialog.Content>
</Dialog.Root>

<DataTable data={s.stocks} {columns} />

<PaginationControls
	count={totalRecords}
	perPage={pageSize}
	bind:page={pageNumber}
/>