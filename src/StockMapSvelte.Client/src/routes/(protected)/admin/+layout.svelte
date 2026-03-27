<script lang="ts">
	import { setContext } from 'svelte';
	import type { IAdminState } from '$lib/Abstractions/IAdminState';

	let adminState = $state<IAdminState>({
		stocks: [],
	})
	setContext('stateAdmin', adminState);

	let { children }: { children: import('svelte').Snippet } = $props();
</script>

{@render children()}
