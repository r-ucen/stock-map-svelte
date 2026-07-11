<script lang="ts">
	import UpdatePassword from '$lib/components/UpdatePassword.svelte';
	import Manage2FA from '$lib/components/Manage2FA.svelte';
	import AddPassword from '$lib/components/AddPassword.svelte';
	import DeleteMyAccount from '$lib/components/DeleteMyAccount.svelte';
	import ExternalLoginManagement from '$lib/components/ExternalLoginManagement.svelte';
</script>

	<div class="mb-6">
		<h1 class="text-2xl font-semibold tracking-tight">Account settings</h1>
		<p class="text-sm text-muted-foreground mt-1">Manage your login, security, and account.</p>
	</div>

	<div class="divide-y divide-border/60">
		<UpdatePassword />
		<AddPassword />
		<ExternalLoginManagement />
		<Manage2FA />
		<DeleteMyAccount />
	</div>