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

{#if status === 'loading'}
	<p>Confirming your email...</p>
{:else if status === 'success'}
	<h1>Email confirmed!</h1>
	<p>Your email has been verified. You can now <a href={resolvedLogin}>sign in</a>.</p>
{:else}
	<h1>Confirmation failed</h1>
	<p>{errorMessage}</p>
{/if}
