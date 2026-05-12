<script lang="ts">
	import { Button } from "$lib/components/ui/button/index.js";
	import * as Card from "$lib/components/ui/card/index.js";
	import * as Field from '$lib/components/ui/field/index.js';
	import * as InputOTP from '$lib/components/ui/input-otp/index.js';
	import { toast } from 'svelte-sonner';

	let recoveryCodeUsed = $state(false)

	let { onSubmit, email, password }:
		{ onSubmit: (code: string, email: string, password: string) => void;
			email: string;
			password: string;
		} = $props();
	let otpValue = $state("");

	function handleSubmit(e: Event) {
		e.preventDefault();
		if (otpValue.length < 6 && !recoveryCodeUsed) {
			toast.error("Please enter all 6 digits.");
			return;
		} else if (otpValue.length < 10 && recoveryCodeUsed){
			toast.error("Please enter the whole code.");
			return;
		}
		onSubmit(otpValue, email, password);
	}
</script>

<Card.Root class="text-center">
	<Card.Header>
		<Card.Title>Two-factor authentication</Card.Title>
		{#if !recoveryCodeUsed}
			<Card.Description>Enter the six digit code from your authentication app.</Card.Description>
		{:else}
		<Card.Description>Enter a recovery code</Card.Description>
		{/if}
	</Card.Header>
	<Card.Content>
		<form onsubmit={handleSubmit}>
			{#if !recoveryCodeUsed}
			<Field.Group>
				<Field.Field>
					<Field.Label class="flex justify-center" for="otp">Verification code</Field.Label>
					<InputOTP.Root class="flex justify-center" bind:value={otpValue} maxlength={6} id="otp" required>
						{#snippet children({ cells })}
							<InputOTP.Group>
								{#each cells.slice(0, 3) as cell (cell)}
									<InputOTP.Slot {cell} />
								{/each}
							</InputOTP.Group>
							<InputOTP.Separator />
							<InputOTP.Group>
								{#each cells.slice(3, 6) as cell (cell)}
									<InputOTP.Slot {cell} />
								{/each}
							</InputOTP.Group>
						{/snippet}
					</InputOTP.Root>
				</Field.Field>
				<Field.Group>
					<Button type="submit">Verify</Button>
					<Field.Description class="text-center">
						Do not have access to auth app? <Button variant="link" onclick={() => {recoveryCodeUsed = true; otpValue = ""}}>Use recovery code</Button>
					</Field.Description>
				</Field.Group>
			</Field.Group>
			{:else}
				<Field.Group>
					<Field.Field>
						<Field.Label class="flex justify-center" for="otp">Recovery code</Field.Label>
						<div class="flex justify-center scale-75 origin-top">
						<InputOTP.Root bind:value={otpValue} maxlength={11} id="otp" required>
							{#snippet children({ cells })}
								<InputOTP.Group>
									{#each cells.slice(0, 5) as cell (cell)}
										<InputOTP.Slot {cell} />
									{/each}
								</InputOTP.Group>

								<InputOTP.Group>
									{#each cells.slice(5, 6) as cell (cell)}
										<InputOTP.Slot {cell} />
									{/each}
								</InputOTP.Group>
								
								<InputOTP.Group>
									{#each cells.slice(6, 11) as cell (cell)}
										<InputOTP.Slot {cell} />
									{/each}
								</InputOTP.Group>
							{/snippet}
						</InputOTP.Root>
						</div>
					</Field.Field>
					<Field.Group>
						<Button type="submit">Verify</Button>
						<Field.Description class="text-center">
							Have access to your authenticator app? <Button variant="link" onclick={() => {recoveryCodeUsed = false; otpValue = ""}}>Use verification code</Button>
						</Field.Description>
					</Field.Group>
				</Field.Group>
			{/if}
		</form>
	</Card.Content>
</Card.Root>
