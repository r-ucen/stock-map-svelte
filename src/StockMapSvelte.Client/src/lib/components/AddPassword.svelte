<script lang="ts">
	import { toast } from 'svelte-sonner';
	import { auth } from '$lib/auth.svelte';
	import {
		Card,
		CardContent,
		CardDescription,
		CardFooter,
		CardHeader,
		CardTitle
	} from '$lib/components/ui/card/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Button } from '$lib/components/ui/button/index.js';
	import type { IState } from '$lib/Abstractions/IState';
	import { getContext } from 'svelte';
	import { Badge } from '$lib/components/ui/badge';
	import BadgeCheckIcon from "@lucide/svelte/icons/badge-check";

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

<Card>
	<CardHeader>
		<CardTitle>
			<span class="pr-2">Add Password</span>
			{#if accountHasPassword}
				<Badge variant="destructive">
					<BadgeCheckIcon />
					Password already configured
				</Badge>
			{/if}
		</CardTitle>
		<CardDescription>
			This action will be permanent.
			New password must be at least 6 characters long, contain uppercase letter, number and a special symbol.
		</CardDescription>
	</CardHeader>
	<CardContent>
		<form id="add-password-form" onsubmit={setPassword} class="space-y-4">
			<div class="space-y-2">
				<Label for="new-password">New Password</Label>
				<Input disabled={accountHasPassword} id="new-password" type="password" bind:value={newPassword} required />
			</div>
			<div class="space-y-2">
				<Label for="confirm-password">Confirm New Password</Label>
				<Input disabled={accountHasPassword} id="confirm-password" type="password" bind:value={confirmPassword} required />
			</div>
		</form>
	</CardContent>
	<CardFooter>
		<Button type="submit" form="add-password-form" disabled={isPasswordSubmitting || accountHasPassword}>
			Set Password
		</Button>
	</CardFooter>
</Card>