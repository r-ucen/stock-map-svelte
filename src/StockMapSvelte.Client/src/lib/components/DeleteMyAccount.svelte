<script lang="ts">

	import { apiFetch } from '$lib/apiFetch';
	import { toast } from 'svelte-sonner';
	import { buttonVariants } from '$lib/components/ui/button';
	import { Card, CardContent, CardHeader, CardTitle } from '$lib/components/ui/card';
	import * as AlertDialog from "$lib/components/ui/alert-dialog/index.js";
	import { goto } from '$app/navigation';
	import { resolve } from '$app/paths';

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

<Card>
	<CardHeader>
		<CardTitle>
			<span class="pr-2">Delete My Account</span>
		</CardTitle>
	</CardHeader>

	<CardContent>
			<AlertDialog.Root bind:open={confirmOpen}>
				<AlertDialog.Trigger class={buttonVariants({ variant: "destructive" })}>
					Delete
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
	</CardContent>
</Card>