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
	import { createMultipleStocks, createStock } from '$lib/components/Stocks/stockActions';
	import { apiFetch } from '$lib/apiFetch';
	import PaginationControls from '$lib/components/PaginationControls.svelte';
	import { Spinner } from "$lib/components/ui/spinner/index.js";
	import * as InputGroup from "$lib/components/ui/input-group/index.js";

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

	// create single stock
	let open = $state(false);
	let isBeingProcessed = $state(false);
	let stockTickerBeingCreated = $state("");

	// create multiple stocks
	let multipleOpen = $state(false);
	let isMultipleBeingProcessed = $state(false);
	let stockTickersBeingCreated = $state("");
	
	let stockSearch = "";
	let stockSearchState = $state("");

	$effect(() => {
		async function fetchPagedStocks() {
			try {
				const params = new URLSearchParams({
					PageNumber: pageNumber.toString(),
					PageSize: pageSize.toString(),
					Search: stockSearchState
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

	function resetCreateMultipleForm() {
		stockTickersBeingCreated = "";
	}

	async function handleCreateMultipleSubmit(e: SubmitEvent){
		const tickers = stockTickersBeingCreated
			.split("\n")
			.map(x => x.trim().toUpperCase())
			.filter(x => x.length > 0);

		if (tickers.length === 0) {
			toast.error("No tickers provided");
			return;
		}
		
		e.preventDefault();
		isMultipleBeingProcessed = true;
		const result = await createMultipleStocks(s, tickers);

		if (!result.success) {
			toast.error(result.error ?? "An error occurred while creating new stocks");
		} else {
			const createdCount = result.data?.createdStocks?.length ?? 0;
			const failed = result.data?.failedToCreateStocks ?? [];

			resetCreateMultipleForm();
			multipleOpen = false;
			pageNumber = 1;

			if (failed.length === 0) {
				toast.success(`Successfully created ${createdCount} stocks`);
			} else {
				
				for (const f of failed) {
					toast.error(
						`${f.ticker}: ${f.error}`
					);
				}
				
				toast.success(
					`Created ${createdCount}, ${failed.length} failed`
				);
			}
		}
		
		isMultipleBeingProcessed = false;
	}
</script>

<div class="flex gap-2">
	<Button variant="outline" size="sm" onclick={() => open = true}>
		<Plus /> Create Stock
	</Button>

	<Button variant="outline" size="sm" onclick={() => multipleOpen = true}>
		<Plus /> Create Multiple Stocks
	</Button>
</div>

<InputGroup.Root>
	<InputGroup.Input placeholder="Type to search..." bind:value={stockSearch} />
	<InputGroup.Addon align="inline-end">
		<InputGroup.Button variant="secondary" onclick={() => stockSearchState = stockSearch}>Search</InputGroup.Button>
	</InputGroup.Addon>
</InputGroup.Root>

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
				>{#if isBeingProcessed}
					<Spinner />
					Submitting...
				{:else}
					Submit
				{/if}
				</Button>
			</Dialog.Footer>
		</form>
	</Dialog.Content>
</Dialog.Root>

<Dialog.Root bind:open={multipleOpen}>
	<Dialog.Content class="sm:max-w-[425px]" onInteractOutside={() => resetCreateMultipleForm()}>
		<form onsubmit={handleCreateMultipleSubmit}>
			<Dialog.Header>
				<Dialog.Title>Create Multiple Stocks</Dialog.Title>
			</Dialog.Header>
			
			<div class="grid gap-4 py-4">
				<div class="grid gap-2">
					<Label for="tickers">
						Ticker Symbols (each on a new line)
					</Label>

					<textarea
						id="tickers"
						class="border rounded-md p-2 min-h-[200px]"
						bind:value={stockTickersBeingCreated}
						disabled={isMultipleBeingProcessed}
					/>
				</div>
			</div>
			
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: "outline" })} onclick={resetCreateMultipleForm}>
					Cancel
				</Dialog.Close>
				<Button
					type="submit"
					disabled={isMultipleBeingProcessed}
				>
					{#if isMultipleBeingProcessed}
						<Spinner />
						Submitting...
						{:else}
						Submit
						{/if}
					</Button>
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