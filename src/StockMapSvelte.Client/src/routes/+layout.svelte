<script lang="ts">
	import './layout.css';
	import favicon from '$lib/assets/favicon.svg';
	import { onMount } from 'svelte';
	import { auth } from '$lib/auth.svelte';
	import { Toaster } from "$lib/components/ui/sonner/index.js";

	let { children } = $props();
	
	onMount(() => {
		auth.checkAuth();
	});
	
</script>

<svelte:head><link rel="icon" href={favicon} /></svelte:head>
<Toaster />
{@render children()}
