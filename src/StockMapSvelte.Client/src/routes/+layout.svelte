<script lang="ts">
	import './layout.css';
	import favicon from '$lib/assets/favicon.svg';
	import { onMount } from 'svelte';
	import { auth } from '$lib/auth.svelte';

	let { children } = $props();
	
	onMount(() => {
		auth.checkAuth();
	});
	
</script>

<svelte:head><link rel="icon" href={favicon} /></svelte:head>
{@render children()}
