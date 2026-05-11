<script lang="ts">
	import { toast } from 'svelte-sonner';
	import { auth } from '$lib/auth.svelte';
	import { CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '$lib/components/ui/card/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Button } from '$lib/components/ui/button/index.js';

	let oldPassword = $state('');
	let newPassword = $state('');
	let confirmPassword = $state('');
	let isPasswordSubmitting = $state(false);

	let arePasswordsMatching = $derived(newPassword === confirmPassword)
	let passwordsMeetRequirements = $derived(/^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/.test(newPassword))

	async function updatePassword(e: Event) {
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
			const res = await auth.changePassword(oldPassword, newPassword);

			if (res.success) {
				toast.success('Password updated successfully');
				oldPassword = '';
				newPassword = '';
				confirmPassword = '';
			} else {
				if (res.errors) {
					res.errors.forEach((err: string) => toast.error(err));
				} else {
					toast.error('Failed to update password');
				}
			}
		} catch {
			toast.error('An error occurred');
		} finally {
			isPasswordSubmitting = false;
		}
	}
</script>

<CardHeader>
	<CardTitle>Change Password</CardTitle>
	<CardDescription>New password must be at least 6 characters long, contain uppercase letter, number and a special symbol.
	</CardDescription>
</CardHeader>
<CardContent>
	<form id="password-form" onsubmit={updatePassword} class="space-y-4">
		<div class="space-y-2">
			<Label for="old-password">Current Password</Label>
			<Input id="old-password" type="password" bind:value={oldPassword} required />
		</div>
		<div class="space-y-2">
			<Label for="new-password">New Password</Label>
			<Input id="new-password" type="password" bind:value={newPassword} required />
		</div>
		<div class="space-y-2">
			<Label for="confirm-password">Confirm New Password</Label>
			<Input id="confirm-password" type="password" bind:value={confirmPassword} required />
		</div>
	</form>
</CardContent>
<CardFooter>
	<Button type="submit" form="password-form" disabled={isPasswordSubmitting}>
		Update Password
	</Button>
</CardFooter>