<script lang="ts">
	import { page } from '$app/state';
	import type { ITwoFaInfo } from '$lib/Abstractions/ITwoFaInfo';
	import { Badge } from '$lib/components/ui/badge';
	import BadgeCheckIcon from "@lucide/svelte/icons/badge-check";
	import BadgeAlertIcon from "@lucide/svelte/icons/badge-alert";
	import { Card, CardContent, CardHeader, CardTitle } from '$lib/components/ui/card';
	import * as ButtonGroup from "$lib/components/ui/button-group/index.js";
	import { Button } from '$lib/components/ui/button/index.js';
	import { apiFetch } from '$lib/apiFetch';
	import { toast } from 'svelte-sonner';
	import QRCode from '@castlenine/svelte-qrcode';
	import * as AlertDialog from "$lib/components/ui/alert-dialog/index.js";
	import { buttonVariants } from '$lib/components/ui/button/index.js';
	import ArrowRightIcon from "@lucide/svelte/icons/arrow-right";
	import OtpForm2 from '$lib/components/otp-form2.svelte';

	

	const twoFaInfoInitial: ITwoFaInfo = $derived(page.data.twoFaInfo);
	const userEmail = $derived(page.data.user?.email);
	
	let twoFaEnabled = $derived(twoFaInfoInitial.isTwoFactorEnabled);
	
	let otpUri = $state("");
	let sharedKey = $state("");
	let recoveryCodes = $state<string[]>([]);
	let confirming2FaInProgress = $state(false);
	
	let qrCodeScanningInProgress = $state(false);

	let disableConfirmOpen = $state(false);
	
	let showingRecoveryCodes = $state(false);
	
	async function disable2FA() {
		const res = await apiFetch('/manage/2fa', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify({
				enable: false
			})
		});
		
		if (!res.ok){
			toast.error('Error disabling 2FA');
			disableConfirmOpen = false;
			return;
		}

		twoFaEnabled = false;
		toast.success('2FA disabled');
		disableConfirmOpen = false;
	}

	async function enable2FA() {
		const res = await apiFetch('/manage/2fa', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify({
				"enable": null,
				"twoFactorCode": null,
				"resetSharedKey": true,
				"resetRecoveryCodes": true,
				"forgetMachine": true
			})
		});

		if (!res.ok){
			toast.error('Error enabling 2FA');
			return;
		}
		const info: ITwoFaInfo = await res.json();
		
		const issuer = "Equmap";
		
		sharedKey = info.sharedKey;
		recoveryCodes = info.recoveryCodes;
		otpUri = `otpauth://totp/StockPortfolioManager:${encodeURIComponent(userEmail)}?secret=${encodeURIComponent(info.sharedKey)}&issuer=${encodeURIComponent(issuer)}`;

		qrCodeScanningInProgress = true;
	}

	async function handleVerify(code: string) {
		const res = await apiFetch('/manage/2fa', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({
				enable: true,
				twoFactorCode: code,
				resetSharedKey: false,
				resetRecoveryCodes: false,
				forgetMachine: true
			})
		});

		if (res.ok) {
			toast.success("2FA enabled successfully!");
			twoFaEnabled = true;
		} else {
			toast.error("Invalid verification code.");
		}

		confirming2FaInProgress = false;
		showingRecoveryCodes = true;
	}
	
</script>

<Card>
	<CardHeader>
		<CardTitle>
			<span class="pr-2">Two Factor Authentication</span>
		{#if twoFaEnabled}
			<Badge variant="secondary" class="bg-blue-500 text-white dark:bg-blue-600">
				<BadgeCheckIcon />
				Enabled
			</Badge>	
		{:else}
			<Badge variant="destructive">
				<BadgeAlertIcon />
				Disabled
			</Badge>
		{/if}
		</CardTitle>
	</CardHeader>

	<CardContent>
		<ButtonGroup.Root>
			<AlertDialog.Root bind:open={qrCodeScanningInProgress}>
				<Button disabled={twoFaEnabled} variant="outline" onclick={() => enable2FA()} class={buttonVariants({ variant: "outline" })}>
					Enable
				</Button>
				<AlertDialog.Content>
					<AlertDialog.Header>
						<AlertDialog.Title>Scan the following QR Code using your authentication app of choice.</AlertDialog.Title>
						<AlertDialog.Description>
							Or enter this code: {sharedKey}
						</AlertDialog.Description>
					</AlertDialog.Header>
					<div class="container">
						<QRCode data={otpUri} />
					</div>
					<AlertDialog.Footer>
						<AlertDialog.Cancel>Close</AlertDialog.Cancel>
						<AlertDialog.Cancel onclick={() => {qrCodeScanningInProgress = false; confirming2FaInProgress = true}}>
							Next <ArrowRightIcon />
						</AlertDialog.Cancel>
					</AlertDialog.Footer>
				</AlertDialog.Content>
			</AlertDialog.Root>

			<AlertDialog.Root bind:open={disableConfirmOpen}>
				<AlertDialog.Trigger disabled={!twoFaEnabled} class={buttonVariants({ variant: "outline" })}>
					Disable
				</AlertDialog.Trigger>
				<AlertDialog.Content>
					<AlertDialog.Header>
						<AlertDialog.Title>Are you absolutely sure you want to disable 2FA??</AlertDialog.Title>
					</AlertDialog.Header>
					<AlertDialog.Footer>
						<AlertDialog.Cancel>Cancel</AlertDialog.Cancel>
						<AlertDialog.Action onclick={() => disable2FA()}>Continue</AlertDialog.Action>
					</AlertDialog.Footer>
				</AlertDialog.Content>
			</AlertDialog.Root>
		</ButtonGroup.Root>

		<AlertDialog.Root bind:open={confirming2FaInProgress}>
			<AlertDialog.Content>
				<AlertDialog.Header>
					<AlertDialog.Title>Enter the code generated by your authentication app.</AlertDialog.Title>
				</AlertDialog.Header>
				<OtpForm2 onSubmit={handleVerify} />
				<AlertDialog.Footer>
					<AlertDialog.Cancel>Close</AlertDialog.Cancel>
				</AlertDialog.Footer>
			</AlertDialog.Content>
		</AlertDialog.Root>

		<AlertDialog.Root bind:open={showingRecoveryCodes}>
			<AlertDialog.Content>
				<AlertDialog.Header>
					<AlertDialog.Title>Copy and save the following codes in case you lose access to the authenticator application.</AlertDialog.Title>
				</AlertDialog.Header>


				<AlertDialog.Root bind:open={showingRecoveryCodes}>
					<AlertDialog.Content>
						<AlertDialog.Header>
							<AlertDialog.Title>
								Copy and save the following codes in case you lose access to the authenticator application.
							</AlertDialog.Title>
						</AlertDialog.Header>
							<div class="space-y-4">
								<textarea	value={recoveryCodes.join('\n')} readonly	class="w-full min-h-[200px] p-3 border rounded-md font-mono text-sm bg-muted"></textarea>
								<Button type="button"	variant="default"
												onclick={() => { navigator.clipboard.writeText(recoveryCodes.join('\n'));
									toast.success('Codes copied to clipboard');
								}}>
									Copy codes
								</Button>
							</div>
					<AlertDialog.Footer>			<AlertDialog.Cancel>Close</AlertDialog.Cancel>		</AlertDialog.Footer>	</AlertDialog.Content></AlertDialog.Root>
				
				<AlertDialog.Footer>
					<AlertDialog.Cancel>Close</AlertDialog.Cancel>
				</AlertDialog.Footer>
			</AlertDialog.Content>
		</AlertDialog.Root>
	</CardContent>
</Card>