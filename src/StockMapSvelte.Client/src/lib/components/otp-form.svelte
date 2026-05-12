<script lang="ts">
	import { Button } from "$lib/components/ui/button/index.js";
	import * as Card from "$lib/components/ui/card/index.js";
	import * as Field from "$lib/components/ui/field/index.js";
	import * as InputOTP from "$lib/components/ui/input-otp/index.js";
	import type { ComponentProps } from "svelte";

	let { ...props }: ComponentProps<typeof Card.Root> = $props();
</script>

<Card.Root {...props}>
	<Card.Header>
		<Card.Title>Two-factor authentication</Card.Title>
		<Card.Description>Enter the six digit code from your authentication app.</Card.Description>
	</Card.Header>
	<Card.Content>
		<form>
			<Field.Group>
				<Field.Field>
					<Field.Label for="otp">Verification code</Field.Label>
					<InputOTP.Root maxlength={6} id="otp" required>
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
				<Field.Group>
					<Button type="submit">Verify</Button>
				</Field.Group>
			</Field.Group>
		</form>
	</Card.Content>
</Card.Root>
