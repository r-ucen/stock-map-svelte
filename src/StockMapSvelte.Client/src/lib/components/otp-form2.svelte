<script lang="ts">
	import { Button } from '$lib/components/ui/button/index.js';
	import * as Field from '$lib/components/ui/field/index.js';
	import * as InputOTP from '$lib/components/ui/input-otp/index.js';
	import { toast } from 'svelte-sonner';

	
	let { onSubmit }: { onSubmit: (code: string) => void } = $props();
	let otpValue = $state(""); 
	
	function handleSubmit(e: Event) { 
		e.preventDefault();
		if (otpValue.length < 6) {
			toast.error("Please enter all 6 digits.");
			return;
		}
		onSubmit(otpValue);
	}
</script>

<form onsubmit={handleSubmit}>
	<Field.Group>
		<Field.Field>
			<Field.Label class="flex justify-center" for="otp">Verification code</Field.Label>
			<InputOTP.Root class="flex justify-center" bind:value={otpValue} maxlength={6} id="otp" required>
				{#snippet children({ cells })}
					<InputOTP.Group
						class="gap-2.5 *:data-[slot=input-otp-slot]:rounded-md *:data-[slot=input-otp-slot]:border"
					>
						{#each cells as cell (cell)}
							<InputOTP.Slot {cell} />
						{/each}
					</InputOTP.Group>
				{/snippet}
			</InputOTP.Root>
		</Field.Field>
		<Field.Group><Button type="submit">Verify</Button></Field.Group>
	</Field.Group>
</form>