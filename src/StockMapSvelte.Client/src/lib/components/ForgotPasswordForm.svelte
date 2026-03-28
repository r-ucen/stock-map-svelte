<script lang="ts">
	import { cn } from "$lib/utils.js";
	import { Button } from "$lib/components/ui/button/index.js";
	import * as Card from "$lib/components/ui/card/index.js";
	import * as Field from "$lib/components/ui/field/index.js";
	import { Input } from "$lib/components/ui/input/index.js";
	import type { HTMLAttributes } from "svelte/elements";
	import { resolve } from '$app/paths';
	import { auth } from '$lib/auth.svelte';
	import { toast } from 'svelte-sonner';

	let { class: className, ...restProps }: HTMLAttributes<HTMLDivElement> = $props();

	const resolvedLogin = resolve("/login")
	const resolvedPrivacyPolicy = resolve("/privacy-policy");

	let email = $state("");

	let isLoading = $state(false);
	let success = $state(false);
	
	async function handleSubmit() {
		isLoading = true;

		const result = await auth.forgotPassword(email);

		if (result.success) {
			success = true;
		} else {
			toast.error("Failed to send reset link. Please try again later.");
		}
		isLoading = false;
	}
</script>

{#if success}
	<div class="text-center">
		<h1 class="text-2xl">Reset link sent successfully to your email. Check your inbox!</h1>
	</div>
{:else}
	<div class={cn("flex flex-col gap-6", className)} {...restProps}>
		<Card.Root>
			<Card.Header class="text-center">
				<Card.Title class="text-xl">Reset your password</Card.Title>
				<Card.Description>Enter your email address below.</Card.Description>
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
							<Button
								type="submit"
								disabled={isLoading}
							>Reset Password</Button>
							<Field.Description class="text-center">
								Remember your password? <a href={resolvedLogin}>Sign in</a>
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
{/if}