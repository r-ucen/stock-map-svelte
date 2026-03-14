<script lang="ts">
	import EllipsisIcon from "@lucide/svelte/icons/ellipsis";
	import { Button, buttonVariants } from "$lib/components/ui/button/index.js";
	import * as DropdownMenu from "$lib/components/ui/dropdown-menu/index.js";
	import * as Dialog from "$lib/components/ui/dialog/index.js";
	import { Input } from "$lib/components/ui/input/index.js";
	import { Label } from "$lib/components/ui/label/index.js";
	import { deletePortfolio, editPortfolio, getPortfolioById } from '$lib/components/Portfolios/dataTableActions';
	import { getContext } from 'svelte';
	import type { IState } from '$lib/Abstractions/IState';
	import StockLookuper from '$lib/components/Portfolios/stock-lookuper.svelte';
	import * as AlertDialog from "$lib/components/ui/alert-dialog/index.js";
	import { toast } from "svelte-sonner";

	let { id }: { id: string } = $props();
	let open = $state(false);
	let deleteOpen = $state(false);

	const s = getContext<IState>('state');
	
	let isBeingProcessed = $state(false);
	let infoMessage = $state<string | null>(null);
	
	let portfolioIdBeingEdited = $state("");
	let portfolioNameBeingEdited = $state("");
	let portfolioStocksBeingEdited = $state<string[]>([]);
	
	let portfolioIdBeingDeleted = $state("");
	let portfolioNameBeingDeleted = $state("");
	
	
	async function handleEditSubmit(e: SubmitEvent){
		e.preventDefault();
		console.log('Submitting:', portfolioNameBeingEdited);
		infoMessage = null;
		isBeingProcessed = true;
		const result = await editPortfolio(s, portfolioIdBeingEdited, portfolioNameBeingEdited, portfolioStocksBeingEdited);
		if (!result.success) {
			infoMessage = result.error ?? null;
		} else {
			infoMessage = "Successfully edited portfolio";
		}
		isBeingProcessed = false;
	}
	
	function onEditClick(id: string){
		const portfolio = getPortfolioById(s.portfolios, id);
		if (portfolio) {
			portfolioIdBeingEdited = portfolio.portfolioId;
			portfolioNameBeingEdited = portfolio.portfolioName;
			portfolioStocksBeingEdited = [...portfolio.tickerSymbols];
		}
		open = true;
	}
	
	async function handleDeleteSubmit() {
		isBeingProcessed = true;

		let result = await deletePortfolio(s, portfolioIdBeingDeleted);
		
		if (!result.success) {
			toast.error(result.error ?? "An error occurred while deleting the portfolio");
		} else {
			toast.success("Portfolio deleted successfully");
			deleteOpen = false;
			return;
		}
		isBeingProcessed = false;
	}
	
	function onDeleteClick(id: string){
		const portfolio = getPortfolioById(s.portfolios, id);
		if (portfolio) {
			portfolioIdBeingDeleted = portfolio.portfolioId;
			portfolioNameBeingDeleted = portfolio.portfolioName;
		}
		deleteOpen = true;
	}
</script>

<Dialog.Root bind:open>
	<DropdownMenu.Root>
		<DropdownMenu.Trigger>
			{#snippet child({ props })}
				<Button
					{...props}
					variant="ghost"
					size="icon"
					class="relative size-8 p-0"
				>
					<span class="sr-only">Open menu</span>
					<EllipsisIcon />
				</Button>
			{/snippet}
		</DropdownMenu.Trigger>
		<DropdownMenu.Content>
			<DropdownMenu.Group>
				<DropdownMenu.Label>Actions</DropdownMenu.Label>
				<DropdownMenu.Item onclick={() => navigator.clipboard.writeText(id)}>
					Copy portfolio ID
				</DropdownMenu.Item>
			</DropdownMenu.Group>
			<DropdownMenu.Separator />
			<DropdownMenu.Item onclick={() => onEditClick(id)}>Edit</DropdownMenu.Item>
			<DropdownMenu.Item>Delete</DropdownMenu.Item>
		</DropdownMenu.Content>
	</DropdownMenu.Root>

	<Dialog.Content class="sm:max-w-[425px]">
		<form onsubmit={handleEditSubmit}>
			<Dialog.Header>
				<Dialog.Title>Edit Portfolio</Dialog.Title>
				<Dialog.Description>{infoMessage}</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4 py-4">
				<div class="grid gap-2">
					<Label for="name">Portfolio Name</Label>
					<Input
						id="name"
						required
						bind:value={portfolioNameBeingEdited}
						disabled={isBeingProcessed}
					/>
				</div>
				<StockLookuper bind:selectedStocks={portfolioStocksBeingEdited} />
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

<AlertDialog.Root bind:open={deleteOpen}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>Are you absolutely sure you want to delete this portfolio?</AlertDialog.Title>
			<AlertDialog.Description>
				This action cannot be undone. This will permanently delete the portfolio <strong>{portfolioNameBeingDeleted}</strong> and all of its data.
			</AlertDialog.Description>
		</AlertDialog.Header>
		<AlertDialog.Footer>
			<AlertDialog.Cancel>Cancel</AlertDialog.Cancel>
			<AlertDialog.Action onclick={() => { handleDeleteSubmit() }}>Continue</AlertDialog.Action>
		</AlertDialog.Footer>
	</AlertDialog.Content>
</AlertDialog.Root>
