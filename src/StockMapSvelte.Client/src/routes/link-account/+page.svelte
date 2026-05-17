<script lang="ts">
	import { page } from '$app/state';
	import { Field, FieldDescription, FieldGroup, FieldLabel } from '$lib/components/ui/field';
	import { Spinner } from '$lib/components/ui/spinner';
	import { Input } from '$lib/components/ui/input';
	import { Button } from '$lib/components/ui/button';
	import { toast } from 'svelte-sonner';
	import * as Card from "$lib/components/ui/card/index.js";
	import { apiFetch } from '$lib/apiFetch';
	import GalleryVerticalEndIcon from '@lucide/svelte/icons/gallery-vertical-end';
	import { resolve } from '$app/paths';
	import { goto } from '$app/navigation';
	
	let email = $derived(page.url.searchParams.get('email'));
	let password = $state("");
	let isLoading = $state(false);

	const resolvedRoot = resolve('/');

	async function handleSubmit(event: Event) {
		event.preventDefault();
		isLoading = true;
		const result = await apiFetch('/oauth/link-oauth-confirm', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify({ email, password }),
		});
		
		if (!result.ok) {
			toast.error('Failed to link accounts. Please check your password and try again.');
		} else {
			toast.success('Accounts linked successfully!');
			await goto(resolvedRoot)
		}
		isLoading = false;
	}
	
</script>
<div class="bg-muted flex min-h-svh flex-col items-center justify-center gap-6 p-6 md:p-10">
	<div class="flex w-full max-w-sm flex-col gap-6">
		<a href={resolvedRoot} class="flex items-center gap-2 self-center font-medium">
			<div
				class="bg-primary text-primary-foreground flex size-6 items-center justify-center rounded-md"
			>
				<GalleryVerticalEndIcon class="size-4" />
			</div>
			Equmap
		</a>
			<Card.Root class="mx-auto w-full max-w-sm">
				<Card.Header>
					<Card.Title class="text-2xl">We have noticed an account with the same e-mail address exists.</Card.Title>
					<Card.Description>If you wish to link the accounts enter the account's password below.</Card.Description>
				</Card.Header>
				<Card.Content>
					<form onsubmit={handleSubmit}>
						<FieldGroup>
							<Field>
								<div class="flex items-center">
									<FieldLabel for="password-link-account">Password</FieldLabel>
								</div>
								<Input
									id="password-link-account"
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
									Don't want to link the account? <a href="/login">Login with the existing account using a password</a>
								</FieldDescription>
							</Field>
						</FieldGroup>
					</form>
				</Card.Content>
			</Card.Root>
	</div>
</div>