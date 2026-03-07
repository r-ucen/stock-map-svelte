<script lang="ts">
	import { auth } from '$lib/auth.svelte.js';
	import { resolve } from '$app/paths';

	const resolvedLogin = resolve('/login');
</script>

<p>You are logged in as {auth.user?.email}</p>

<a href={resolvedLogin}>login</a>

<button onclick={() => auth.logout()}>
	Logout
</button>