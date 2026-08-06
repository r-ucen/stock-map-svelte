<script lang="ts">
	import { Button } from "$lib/components/ui/button/index.js";

	import { auth } from '$lib/auth.svelte.js';
	function useDifferentAccount() {
		auth.logout();
	}
</script>

<div class="flex min-h-screen items-center justify-center bg-background px-4">
	<div class="text-center">
		<h1 class="text-primary leading-tighter mb-6 text-4xl font-semibold tracking-tight text-balance lg:leading-[1.1] lg:font-semibold xl:text-5xl xl:tracking-tighter max-w-4xl">You have been banned.</h1>
		<Button variant="link" onclick={useDifferentAccount}>Use a different account</Button>
	</div>
</div>