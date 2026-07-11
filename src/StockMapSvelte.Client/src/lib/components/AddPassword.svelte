<script lang="ts">
	import { toast } from 'svelte-sonner';
	import { auth } from '$lib/auth.svelte';
	import { Label } from '$lib/components/ui/label/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Button } from '$lib/components/ui/button/index.js';
	import type { IState } from '$lib/Abstractions/IState';
	import { getContext } from 'svelte';
	import { Badge } from '$lib/components/ui/badge';
	import BadgeAlertIcon from "@lucide/svelte/icons/badge-alert";
	import KeyRoundIcon from "@lucide/svelte/icons/key-round";

	let newPassword = $state('');
	let confirmPassword = $state('');
	let isPasswordSubmitting = $state(false);

	let arePasswordsMatching = $derived(newPassword === confirmPassword)
	let passwordsMeetRequirements = $derived(/^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/.test(newPassword))

	const s = getContext<IState>('state');
	const accountHasPassword = $derived(s.hasPasswordConfigured);

	async function setPassword(e: Event) {
		e.preventDefault();
		
		if (!arePasswordsMatching) {
			toast.error('New passwords do not match');
			return;
		}

		if (!passwordsMeetRequirements) {
			toast.error('Passwords do not meet requirements');
			return;
		}

		isPasswordSubmitting = true;
		try {
			const res = await auth.addPassword(newPassword);

			if (res.success) {
				toast.success('Password added successfully');
				s.hasPasswordConfigured = true;
				newPassword = '';
				confirmPassword = '';
			} else {
				if (res.error) {
					toast.error(res.error);
				} else {
					toast.error('Failed to add password');
				}
			}
		} catch {
			toast.error('An error occurred');
		} finally {
			isPasswordSubmitting = false;
		}
	}
</script>

<div class="py-8 first:pt-0">
	<div class="flex items-start gap-3">
		<div class="flex size-9 shrink-0 items-center justify-center rounded-full bg-muted text-muted-foreground">
			<KeyRoundIcon class="size-4" />
		</div>
		<div class="min-w-0 flex-1">
			<div class="flex flex-wrap items-center gap-2">
				<h2 class="text-base font-semibold tracking-tight">Add password</h2>
				{#if accountHasPassword}
					<Badge variant="destructive">
						<BadgeAlertIcon />
						Password already configured
					</Badge>
				{/if}
			</div>
			<p class="text-sm text-muted-foreground mt-1">
				This action will be permanent. New password must be at least 6 characters long, contain uppercase letter, number and a special symbol.
			</p>

			<form id="add-password-form" onsubmit={setPassword} class="mt-5 max-w-sm space-y-4">
				<div class="space-y-1.5">
					<Label for="new-password" class="text-sm font-medium">New password</Label>
					<Input disabled={accountHasPassword} id="new-password" type="password" bind:value={newPassword} required />
				</div>
				<div class="space-y-1.5">
					<Label for="confirm-password" class="text-sm font-medium">Confirm new password</Label>
					<Input disabled={accountHasPassword} id="confirm-password" type="password" bind:value={confirmPassword} required />
				</div>
			</form>

			<Button type="submit" form="add-password-form" disabled={isPasswordSubmitting || accountHasPassword} class="mt-5">
				Set password
			</Button>
		</div>
	</div>
</div>