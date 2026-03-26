<script lang="ts">
	import DataTable from "./data-table.svelte";
	import { columns } from "./columns.js";
	import { getContext } from 'svelte';
	import { type IPortfolio, type IState } from '$lib/Abstractions/IState';
	import { Button, buttonVariants } from '$lib/components/ui/button';
	import Plus  from "@lucide/svelte/icons/plus";
	import StockLookuper from '$lib/components/Portfolios/stock-lookuper.svelte';
	import { Input } from '$lib/components/ui/input';
	import { Label } from '$lib/components/ui/label';
	import * as Dialog from "$lib/components/ui/dialog/index.js";
	import { createPortfolio } from '$lib/components/Portfolios/portfolioActions';
	import { toast } from 'svelte-sonner';
	import { fetchTreemapData } from '$lib/portfolioFetch';
	
	let s = getContext<IState>('state');
	let { items }: {items: IPortfolio[]} = $props();

	$effect(() => {
		s.portfolios = items;
	});

	let open = $state(false);

	let isBeingProcessed = $state(false);

	let portfolioNameBeingCreated = $state("");
	let portfolioStocksBeingCreated = $state<string[]>([]);

	async function handleCreateSubmit(e: SubmitEvent){
		e.preventDefault();
		isBeingProcessed = true;
		const result = await createPortfolio(s, portfolioNameBeingCreated, portfolioStocksBeingCreated);
		if (!result.success) {
			toast.error(result.error ?? "An error occurred while creating new portfolio");
		} else {
			await fetchTreemapData(s);
			toast.success("Successfully created portfolio");
		}
		isBeingProcessed = false;
	}
</script>

<Button
	variant="outline"
	size="sm"
	onclick={() => open = true}
>
	<Plus /> Create Portfolio
</Button>

<Dialog.Root bind:open>
	<Dialog.Content class="sm:max-w-[425px]">
		<form onsubmit={handleCreateSubmit}>
			<Dialog.Header>
				<Dialog.Title>Create New Portfolio</Dialog.Title>
			</Dialog.Header>
			<div class="grid gap-4 py-4">
				<div class="grid gap-2">
					<Label for="name">Portfolio Name</Label>
					<Input
						id="name"
						required
						bind:value={portfolioNameBeingCreated}
						disabled={isBeingProcessed}
					/>
				</div>
				<StockLookuper bind:selectedStocks={portfolioStocksBeingCreated} />
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: "outline" })}>
					Cancel
				</Dialog.Close>
				<Button
					type="submit"
					disabled={isBeingProcessed}
				>Save changes</Button>
			</Dialog.Footer>
		</form>
	</Dialog.Content>
</Dialog.Root>

<DataTable data={s.portfolios} {columns}  />