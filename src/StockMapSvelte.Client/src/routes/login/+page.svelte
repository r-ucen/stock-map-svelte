<script lang="ts">
	import LoginForm from "$lib/components/login-form.svelte";
	import { resolve } from '$app/paths';
	
	const resolvedAccount = resolve('/account');
</script>

<div class="flex h-screen w-full items-center justify-center px-4">
	<LoginForm />
</div>

<a href={resolvedAccount}>account</a>
