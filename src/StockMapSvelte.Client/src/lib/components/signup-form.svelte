<script lang="ts">
	import { cn } from "$lib/utils.js";
	import { Button } from "$lib/components/ui/button/index.js";
	import * as Card from "$lib/components/ui/card/index.js";
	import * as Field from "$lib/components/ui/field/index.js";
	import { Input } from "$lib/components/ui/input/index.js";
	import type { HTMLAttributes } from "svelte/elements";
	import { resolve } from '$app/paths';
	import { auth } from '$lib/auth.svelte';

	let { class: className, ...restProps }: HTMLAttributes<HTMLDivElement> = $props();
	
	const resolvedLogin = resolve("/login")
	const resolvedPrivacyPolicy = resolve("/privacy-policy");
	
	let email = $state("");
	let password = $state("");
	let confirmPassword = $state("");

	let isLoading = $state(false);
	let infoMessage = $state<string | null>(null);
	let errors = $state<string[] | undefined>(undefined);
	
	let arePasswordsMatching = $derived(password === confirmPassword)

	async function handleSubmit() {
		isLoading = true;
		
		const result = await auth.register(email, password);
		
		if (result.success) {
			infoMessage = 'Check your email for a confirmation link to complete your registration';
		} else {
			errors = result.errors
		}
		isLoading = false;
	}
</script>

<p>{infoMessage}</p>

{#if errors}
	{#each errors as error, i (i)}
		<p>{error}</p>
	{/each}
{/if}

<div class={cn("flex flex-col gap-6", className)} {...restProps}>
	<Card.Root>
		<Card.Header class="text-center">
			<Card.Title class="text-xl">Create your account</Card.Title>
			<Card.Description>Enter your email below to create your account</Card.Description>
		</Card.Header>
		<Card.Content>
			<form onsubmit={handleSubmit}>
				<Field.Group>
					<Field.Field>
						<Field.Label for="email">Email</Field.Label>
						<Input
							id="email"
							type="email"
							placeholder="m@example.com"
							required
							bind:value={email}
							disabled={isLoading}
						/>
					</Field.Field>
					<Field.Field>
						<Field.Field class="grid grid-cols-2 gap-4">
							<Field.Field>
								<Field.Label for="password">Password</Field.Label>
								<Input
									id="password"
									type="password"
									required
									bind:value={password}
									disabled={isLoading}
								/>
							</Field.Field>
							<Field.Field>
								<Field.Label for="confirm-password">Confirm Password</Field.Label>
								<Input
									id="confirm-password"
									type="password"
									required
									bind:value={confirmPassword}
									disabled={isLoading}
								/>
							</Field.Field>
						</Field.Field>
						<Field.Description>
							Must be at least 6 characters long, contain uppercase letter, number and a special symbol.
						</Field.Description>
					</Field.Field>
					<Field.Field>
						<Button
							type="submit"
							disabled={!arePasswordsMatching || isLoading}
						>Create Account</Button>
						<Field.Description class="text-center">
							Already have an account? <a href={resolvedLogin}>Sign in</a>
						</Field.Description>
					</Field.Field>
				</Field.Group>
			</form>
		</Card.Content>
	</Card.Root>
	<Field.Description class="px-6 text-center">
		By clicking continue, you agree to our <a href={resolvedPrivacyPolicy}>Privacy Policy</a>.
	</Field.Description>
</div>
