<script lang="ts">
	import EllipsisIcon from "@lucide/svelte/icons/ellipsis";
	import { Button } from "$lib/components/ui/button";
	import * as DropdownMenu from "$lib/components/ui/dropdown-menu";
	import {
		deleteUser,
		getUserById,
	} from '$lib/components/Users/userActions';
	import { getContext } from 'svelte';
	import * as AlertDialog from "$lib/components/ui/alert-dialog";
	import { toast } from "svelte-sonner";
	import type { IAdminState } from '$lib/Abstractions/IAdminState';

	let { id }: { id: string } = $props();
	let deleteOpen = $state(false);

	const s = getContext<IAdminState>('stateAdmin');

	let isBeingProcessed = $state(false);

	let userIdBeingDeleted = $state("");
	let userEmailBeingDeleted = $state("");

	async function handleDeleteSubmit() {
		isBeingProcessed = true;

		let result = await deleteUser(s, userIdBeingDeleted);

		if (!result.success) {
			toast.error(result.error ?? "An error occurred while deleting the user");
		} else {
			toast.success("User deleted successfully");
			deleteOpen = false;
		}
		isBeingProcessed = false;
	}

	function onDeleteClick(id: string){
		const user = getUserById(s.users, id);
		if (user) {
			userIdBeingDeleted = user.id;
			userEmailBeingDeleted = user.email;
		}
		deleteOpen = true;
	}
</script>

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
				Copy user ID
			</DropdownMenu.Item>
		</DropdownMenu.Group>
		<DropdownMenu.Separator />
		<DropdownMenu.Item onclick={() => onDeleteClick(id)}>Delete</DropdownMenu.Item>
	</DropdownMenu.Content>
</DropdownMenu.Root>


<AlertDialog.Root bind:open={deleteOpen}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>Are you absolutely sure you want to delete this user?</AlertDialog.Title>
			<AlertDialog.Description>
				This action cannot be undone. This will permanently delete the user <strong>{userEmailBeingDeleted}</strong> and all of its data.
			</AlertDialog.Description>
		</AlertDialog.Header>
		<AlertDialog.Footer>
			<AlertDialog.Cancel>Cancel</AlertDialog.Cancel>
			<AlertDialog.Action onclick={() => { handleDeleteSubmit() }}>Continue</AlertDialog.Action>
		</AlertDialog.Footer>
	</AlertDialog.Content>
</AlertDialog.Root>

