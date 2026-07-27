<script lang="ts">
	import type { IState } from '$lib/Abstractions/IState';
	import { getContext } from 'svelte';
	import type { Login } from '$lib/Abstractions/ILogin';
	import { Button } from '$lib/components/ui/button';
	import Trash2 from "@lucide/svelte/icons/trash-2";
	import { apiFetch } from '$lib/apiFetch';
	import { toast } from 'svelte-sonner';
	import { goto } from '$app/navigation';
	import { resolve } from '$app/paths';

	import * as AlertDialog from "$lib/components/ui/alert-dialog/index.js";

	const resolvedLogin = resolve('/login');
	
	let showConfirmDialog = $state(false);

	const s = getContext<IState>('state');
	const googleLogin: Login = {
		loginProvider: 'GoogleOpenIdConnect',
		providerDisplayName: 'Google OpenIdConnect',
	};
	const hasGoogleLoginConfigured = $derived(
		s.externalLogins.some(login => login.loginProvider === googleLogin.loginProvider)
	);
	
	async function removeGoogleExternalLogin() {
		const res = await apiFetch('/account/remove-google-external-login', {
			method: 'DELETE',
			headers: {
				'Content-Type': 'application/json'
			}
		});

		if (!res.ok){
			toast.error('Google external login is not configured or an error occurred');
		} else {
			toast.success('Google external login removed successfully');
			await goto(resolvedLogin);
		}

		showConfirmDialog = false;
	}
</script>

<AlertDialog.Root bind:open={showConfirmDialog}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>Are you absolutely sure you want to remove Google login?</AlertDialog.Title>
		</AlertDialog.Header>
		<AlertDialog.Footer>
			<AlertDialog.Cancel>Cancel</AlertDialog.Cancel>
			<AlertDialog.Action onclick={removeGoogleExternalLogin}>Continue</AlertDialog.Action>
		</AlertDialog.Footer>
	</AlertDialog.Content>
</AlertDialog.Root>

<Button variant="outline" size="sm" disabled={!hasGoogleLoginConfigured} onclick={() => { showConfirmDialog = true}}>
	<Trash2 />
</Button>