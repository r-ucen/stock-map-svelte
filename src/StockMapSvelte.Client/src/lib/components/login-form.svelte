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
	import { PUBLIC_API_BASE_URL } from '$env/static/public';

	let { id = "login-form" } = $props();

	let email = $state("");
	let password = $state("");
	let twoFactorCode = $state<string | null>(null);
	let twoFactorRecoveryCode = $state<string | null>(null);
	let is2FaVisible = $state(false);
	let isLoading = $state(false);
	
	function loginWithGoogle(){
		let returnUrl = encodeURIComponent(`${window.location.origin}/portfolios`);
		window.location.href = `${PUBLIC_API_BASE_URL}/oauth/google-login?returnUrl=${returnUrl}`;
	}

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
						<Button variant="outline" class="w-full" onclick={loginWithGoogle}>
							<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
								<path
									d="M12.48 10.92v3.28h7.84c-.24 1.84-.853 3.187-1.787 4.133-1.147 1.147-2.933 2.4-6.053 2.4-4.827 0-8.6-3.893-8.6-8.72s3.773-8.72 8.6-8.72c2.6 0 4.507 1.027 5.907 2.347l2.307-2.307C18.747 1.44 16.133 0 12.48 0 5.867 0 .307 5.387.307 12s5.56 12 12.173 12c3.573 0 6.267-1.173 8.373-3.36 2.16-2.16 2.84-5.213 2.84-7.667 0-.76-.053-1.467-.173-2.053H12.48z"
									fill="currentColor"
								/>
							</svg>
							Login with Google
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