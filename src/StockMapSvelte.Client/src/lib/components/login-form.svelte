<script lang="ts">
	import { Button } from "$lib/components/ui/button/index.js";
	import * as Card from "$lib/components/ui/card/index.js";
	import { Input } from "$lib/components/ui/input/index.js";
	import {
		FieldGroup,
		Field,
		FieldLabel,
		FieldDescription,
	} from "$lib/components/ui/field/index.js";
	import { auth } from '$lib/auth.svelte';
	import { Spinner } from '$lib/components/ui/spinner';
	import { toast } from 'svelte-sonner';
	
	let { id = "login-form" } = $props();

	let email = $state("");
	let password = $state("");
	let isLoading = $state(false);

	async function handleSubmit() {
		isLoading = true;
		const result = await auth.login(email, password);
		if (!result.success) {
			toast.error('Invalid credentials');
		}
		isLoading = false;
	}
</script>


<Card.Root class="mx-auto w-full max-w-sm">
	<Card.Header>
		<Card.Title class="text-2xl">Login</Card.Title>
		<Card.Description>Enter your email below to login to your account</Card.Description>
	</Card.Header>
	<Card.Content>
		<form onsubmit={handleSubmit}>
			<FieldGroup>
				<Field>
					<FieldLabel for="email-{id}">Email</FieldLabel>
					<Input
						id="email-{id}"
						type="email"
						placeholder="m@example.com"
						required
						bind:value={email}
						disabled={isLoading} />
				</Field>
				<Field>
					<div class="flex items-center">
						<FieldLabel for="password-{id}">Password</FieldLabel>
						<a href="/forgot-password" class="ms-auto inline-block text-sm underline">
							Forgot your password?
						</a>
					</div>
					<Input
						id="password-{id}"
						type="password"
						required
						bind:value={password}
						disabled={isLoading}/>
				</Field>
				<Field>
					<Button
						type="submit"
						class="w-full"
						disabled={isLoading}>
						{#if isLoading}
							<Spinner />
							Loading...
						{:else}
							Login
						{/if}
					</Button>
					<FieldDescription class="text-center">
						Don't have an account? <a href="/register">Sign up</a>
					</FieldDescription>
				</Field>
			</FieldGroup>
		</form>
	</Card.Content>
</Card.Root>
