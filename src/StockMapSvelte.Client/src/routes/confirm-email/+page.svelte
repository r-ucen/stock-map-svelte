<script lang="ts">
	import { page } from '$app/state';
	import { auth } from '$lib/auth.svelte';
	import { resolve } from '$app/paths';

	let status = $state<'loading' | 'success' | 'error'>('loading');
	let errorMessage = $state('');

	const resolvedLogin = resolve('/login');

	$effect(() => {
		const userId = page.url.searchParams.get('userId');
		const code = page.url.searchParams.get('code');

		if (!userId || !code) {
			status = 'error';
			errorMessage = 'Missing userId or confirmation code.';
			return;
		}

		confirmEmail(userId, code);
	});

	async function confirmEmail(userId: string, code: string) {
		const result = await auth.confirmEmail(userId, code);

		if (result.success) {
			status = 'success';
		} else {
			status = 'error';
			errorMessage = 'Email confirmation failed.';
		}
	}
</script>
<div class="flex h-screen w-full items-center justify-center px-4">
	<div class="flex flex-col items-center justify-center space-y-4 text-center">
		{#if status === 'loading'}
			<p class="text-lg text-muted-foreground">Confirming your email...</p>
		{:else if status === 'success'}
			<h1 class="text-3xl">Email confirmed!</h1>
			<p class="text-muted-foreground">Your email has been verified.</p>
			<p class="text-muted-foreground">You can now <a href={resolvedLogin} class="text-primary font-medium underline underline-offset-4">sign in</a>.</p>
		{:else}
			<h1 class="text-3xl text-destructive">Confirmation failed</h1>
			<p class="text-muted-foreground">{errorMessage}</p>
		{/if}
	</div>
</div>