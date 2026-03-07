<script lang="ts">
	import { auth } from '$lib/auth.svelte';
	import { resolve } from '$app/paths';
	import { goto } from '$app/navigation';
	import { Spinner } from '$lib/components/ui/spinner';

	$effect(() => {
		if (auth.isInitialized && !auth.isAuthenticated) {
			goto(resolve('/login'));
		}
	});

	let { children } = $props();
</script>

{#if !auth.isInitialized}
	<Spinner />
{:else if auth.isAuthenticated}
	{@render children()}
{/if}
