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
	import OtpForm from '$lib/components/otp-form.svelte';

	let { id = "login-form" } = $props();

	let email = $state("");
	let password = $state("");
	let twoFactorCode = $state<string | null>(null);
	let twoFactorRecoveryCode = $state<string | null>(null);
	let is2FaVisible = $state(false);
	let isLoading = $state(false);

	async function handleSubmit() {
		isLoading = true;
		const result = await auth.login(email, password, null, null);
		if (result.status == 401 && (result.error === 'RequiresTwoFactor' || result.requiresTwoFactor == true)) {
			is2FaVisible = true;
		}
		else if (!result.success) {
			toast.error(result.error);
		}
		isLoading = false;
	}

	async function handle2FaSubmit(code: string, email: string, password: string) {
		if (code.length === 6){
			twoFactorCode = code;
		} else {
			twoFactorRecoveryCode = code;
		}
		
		const result = await auth.login(email, password, twoFactorCode, twoFactorRecoveryCode);
		
		if (!result.success) {
			toast.error("Invalid code");
			is2FaVisible = true;
		} else {
			is2FaVisible = false;
		}
		twoFactorCode = null;
		twoFactorRecoveryCode = null;
	}
</script>



{#if is2FaVisible}
	<OtpForm onSubmit={handle2FaSubmit} email={email} password={password} />
{:else}
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
{/if}