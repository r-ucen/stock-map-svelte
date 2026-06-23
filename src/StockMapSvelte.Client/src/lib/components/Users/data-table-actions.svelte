<script lang="ts">
	import EllipsisIcon from "@lucide/svelte/icons/ellipsis";
	import { Button } from "$lib/components/ui/button";
	import * as DropdownMenu from "$lib/components/ui/dropdown-menu";
	import {
		deleteUser,
		getUserById, updateUserRoles
	} from '$lib/components/Users/userActions';
	import { getContext } from 'svelte';
	import * as AlertDialog from "$lib/components/ui/alert-dialog";
	import { toast } from "svelte-sonner";
	import type { IAdminState } from '$lib/Abstractions/IAdminState';
	import Trash2 from '@lucide/svelte/icons/trash-2';
	import UserKey  from '@lucide/svelte/icons/user-key';

	let { id, roles }: { id: string, roles: string[] } = $props();
	let deleteOpen = $state(false);

	const s = getContext<IAdminState>('stateAdmin');

	let isBeingProcessed = $state(false);
	let isRoleUpdating = $state(false);

	let userIdBeingDeleted = $state("");
	let userEmailBeingDeleted = $state("");

	let hasAdminRole = $derived(roles.includes('Admin'));
	let hasManagerRole = $derived(roles.includes('Manager'));

	async function handleDeleteSubmit() {
		isBeingProcessed = true;

		let result = await deleteUser(s, userIdBeingDeleted);

		if (!result.success) {
			toast.error(result.error ?? "An error occurred while deleting the user");
		} else {
			toast.success("User deleted successfully");
			deleteOpen = false;
		}
		userEmailBeingDeleted = ""
		userIdBeingDeleted = ""
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

	async function handleRoleToggle(roleName: string, shouldHaveRole: boolean) {
		if (isRoleUpdating) { return; }
		isRoleUpdating = true;

		const newRoles = shouldHaveRole
			? [...roles, roleName]
			: roles.filter(r => r !== roleName);

		const result = await updateUserRoles(s, id, newRoles);

		if (!result.success) {
			toast.error(result.error ?? `Failed to update ${roleName} role`);
		} else {
			toast.success(`User roles updated successfully`);
		}
		isRoleUpdating = false;
		s.refreshUsers++;
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
		</DropdownMenu.Group>
		<DropdownMenu.Separator />

		<DropdownMenu.Sub>

			<DropdownMenu.SubTrigger>
				<UserKey />
				Roles
			</DropdownMenu.SubTrigger>

			<DropdownMenu.SubContent>
				<DropdownMenu.Group>
					<DropdownMenu.CheckboxItem bind:checked={hasAdminRole} onCheckedChange={(isNowChecked) => handleRoleToggle("Admin", isNowChecked)}>
						Admin
					</DropdownMenu.CheckboxItem>
					<DropdownMenu.CheckboxItem bind:checked={hasManagerRole} onCheckedChange={(isNowChecked) => handleRoleToggle("Manager", isNowChecked)}>
						Manager
					</DropdownMenu.CheckboxItem>
				</DropdownMenu.Group>
			</DropdownMenu.SubContent>
		</DropdownMenu.Sub>
		
		<DropdownMenu.Item class="text-destructive" onclick={() => onDeleteClick(id)}>
			<Trash2 class="text-destructive" />
			Delete
		</DropdownMenu.Item>
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

