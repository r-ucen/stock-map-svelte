<script lang="ts">

	import { apiFetch } from '$lib/apiFetch';
	import { toast } from 'svelte-sonner';
	import { buttonVariants } from '$lib/components/ui/button';
	import * as AlertDialog from "$lib/components/ui/alert-dialog/index.js";
	import { goto } from '$app/navigation';
	import { resolve } from '$app/paths';
	import Trash2Icon from "@lucide/svelte/icons/trash-2";

	let confirmOpen = $state(false);
	const resolvedLogin = resolve('/login');

	async function deleteMyAccount() {
		const res = await apiFetch('/account/me', {
			method: 'DELETE',
			headers: {
				'Content-Type': 'application/json'
			}
		});

		if (!res.ok){
			toast.error('Error deleting account');
		} else {
			toast.success('Account successfully deleted');
			await goto(resolvedLogin);
		}
		confirmOpen = false;
	}
	
</script>

<div class="py-8">
	<div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
		<div class="flex items-start gap-3">
			<div class="flex size-9 shrink-0 items-center justify-center rounded-full bg-destructive/10 text-destructive">
				<Trash2Icon class="size-4" />
			</div>
			<div>
				<h2 class="text-base font-semibold tracking-tight text-destructive">Delete account</h2>
				<p class="text-sm text-muted-foreground mt-1">Permanently remove your account and all of its data. This can't be undone.</p>
			</div>
		</div>

		<div class="ml-12 ">
			<AlertDialog.Root bind:open={confirmOpen}>
				<AlertDialog.Trigger class={buttonVariants({ variant: "destructive" })}>
					Delete account
				</AlertDialog.Trigger>
				<AlertDialog.Content>
					<AlertDialog.Header>
						<AlertDialog.Title>Click "delete" if you are sure you want to delete your account.</AlertDialog.Title>
						<AlertDialog.Description>
							This action cannot be undone.
						</AlertDialog.Description>
					</AlertDialog.Header>
					<AlertDialog.Footer>
						<AlertDialog.Cancel>Close</AlertDialog.Cancel>
						<AlertDialog.Action onclick={deleteMyAccount}>
							Delete
						</AlertDialog.Action>
					</AlertDialog.Footer>
				</AlertDialog.Content>
			</AlertDialog.Root>
		</div>
	</div>
</div>