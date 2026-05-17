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
	import { PUBLIC_API_BASE_URL } from '$env/static/public';

	let { class: className, ...restProps }: HTMLAttributes<HTMLDivElement> = $props();
	
	const resolvedLogin = resolve("/login")
	const resolvedPrivacyPolicy = resolve("/privacy-policy");
	
	let email = $state("");
	let password = $state("");
	let confirmPassword = $state("");

	let isLoading = $state(false);
	let success = $state(false);
	
	let arePasswordsMatching = $derived(password === confirmPassword)
	let passwordsMeetRequirements = $derived(arePasswordsMatching && /^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/.test(password))

	async function handleSubmit() {
		isLoading = true;
		
		if (!passwordsMeetRequirements) {
			toast.error("Passwords do not meet requirements");
			isLoading = false;
			return;
		}
		
		const result = await auth.register(email, password);
		
		if (result.success) {
			success = true;
		} else {
			result?.errors?.forEach(error => toast.error(error));
		}
		isLoading = false;
	}

	function signupWithGoogle(){
		let returnUrl = encodeURIComponent(`${window.location.origin}/map`);
		window.location.href = `${PUBLIC_API_BASE_URL}/oauth/google-login?returnUrl=${returnUrl}`;
	}
</script>

{#if success}
	<div class="text-center">
		<h1 class="text-2xl">Account created successfully!</h1>
		<h2 class="text-lg">Check your email for a confirmation link to complete your registration</h2>
		<h2 class="text-lg"><a href="/resend-email-confirmation" class="text-primary font-medium underline underline-offset-4">Resend email confirmation</a></h2>
	</div>
{:else}
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
							<Button variant="outline" type="button" onclick={signupWithGoogle}>Sign up with Google</Button>
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
{/if}