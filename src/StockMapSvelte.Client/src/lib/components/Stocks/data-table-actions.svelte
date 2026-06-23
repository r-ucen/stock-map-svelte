<script lang="ts">
	import EllipsisIcon from "@lucide/svelte/icons/ellipsis";
	import { Button, buttonVariants } from "$lib/components/ui/button";
	import * as DropdownMenu from "$lib/components/ui/dropdown-menu";
	import * as Dialog from "$lib/components/ui/dialog";
	import { Input } from "$lib/components/ui/input";
	import { Label } from "$lib/components/ui/label";
	import {
		deleteStock,
		editStock,
		getStockById,
	} from '$lib/components/Stocks/stockActions';
	import { getContext } from 'svelte';
	import * as AlertDialog from "$lib/components/ui/alert-dialog";
	import { toast } from "svelte-sonner";
	import type { IAdminState } from '$lib/Abstractions/IAdminState';
	import Trash2 from "@lucide/svelte/icons/trash-2";
	import Pen from "@lucide/svelte/icons/pen";

	let { id }: { id: string } = $props();
	let open = $state(false);
	let deleteOpen = $state(false);

	const s = getContext<IAdminState>('stateAdmin');

	let isBeingProcessed = $state(false);

	let stockIdBeingEdited = $state("");
	let tickerSymbolBeingEdited = $state("");

	let stockIdBeingDeleted = $state("");
	let tickerSymbolBeingDeleted = $state("");


	async function handleEditSubmit(e: SubmitEvent){
		e.preventDefault();
		isBeingProcessed = true;
		const result = await editStock(s, stockIdBeingEdited, tickerSymbolBeingEdited);
		if (!result.success) {
			toast.error(result.error ?? "An error occurred while editing the stock");
		} else {
			toast.success("Successfully edited stock");
		}
		isBeingProcessed = false;
	}

	function onEditClick(id: string){
		const stock = getStockById(s.stocks, id);
		if (stock) {
			stockIdBeingEdited = stock.id;
			tickerSymbolBeingEdited = stock.tickerSymbol;
		}
		open = true;
	}

	async function handleDeleteSubmit() {
		isBeingProcessed = true;

		let result = await deleteStock(s, stockIdBeingDeleted);

		if (!result.success) {
			toast.error(result.error ?? "An error occurred while deleting the stock");
		} else {
			toast.success("Stock deleted successfully");
			deleteOpen = false;
		}
		isBeingProcessed = false;
	}

	function onDeleteClick(id: string){
		const stock = getStockById(s.stocks, id);
		if (stock) {
			stockIdBeingDeleted = stock.id;
			tickerSymbolBeingDeleted = stock.tickerSymbol;
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
			</DropdownMenu.Group>
			<DropdownMenu.Separator />
			<DropdownMenu.Item onclick={() => onEditClick(id)}>
				<Pen />
				Edit
			</DropdownMenu.Item>
			<DropdownMenu.Item class="text-destructive" onclick={() => onDeleteClick(id)}>
				<Trash2 class="text-destructive" />
				Delete
			</DropdownMenu.Item>
		</DropdownMenu.Content>
	</DropdownMenu.Root>

	<Dialog.Content class="sm:max-w-[425px]">
		<form onsubmit={handleEditSubmit}>
			<Dialog.Header>
				<Dialog.Title>Edit Stock</Dialog.Title>
			</Dialog.Header>
			<div class="grid gap-4 py-4">
				<div class="grid gap-2">
					<Label for="name">Ticker Symbol</Label>
					<Input
						id="name"
						required
						bind:value={tickerSymbolBeingEdited}
						disabled={isBeingProcessed}
					/>
				</div>
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
			<AlertDialog.Title>Are you absolutely sure you want to delete this stock?</AlertDialog.Title>
			<AlertDialog.Description>
				This action cannot be undone. This will permanently delete the stock <strong>{tickerSymbolBeingDeleted}</strong> and all of its data.
			</AlertDialog.Description>
		</AlertDialog.Header>
		<AlertDialog.Footer>
			<AlertDialog.Cancel>Cancel</AlertDialog.Cancel>
			<AlertDialog.Action onclick={() => { handleDeleteSubmit() }}>Continue</AlertDialog.Action>
		</AlertDialog.Footer>
	</AlertDialog.Content>
</AlertDialog.Root>
