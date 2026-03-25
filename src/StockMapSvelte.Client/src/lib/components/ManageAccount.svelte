<script lang="ts">
	import { auth } from '$lib/auth.svelte.js';
</script>

<p>You are logged in as {auth.user?.email}</p>
