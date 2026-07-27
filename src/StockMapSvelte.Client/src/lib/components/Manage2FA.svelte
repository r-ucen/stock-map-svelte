<script lang="ts">
	import { page } from '$app/state';
	import type { ITwoFaInfo } from '$lib/Abstractions/ITwoFaInfo';
	import { Badge } from '$lib/components/ui/badge';
	import BadgeCheckIcon from "@lucide/svelte/icons/badge-check";
	import BadgeAlertIcon from "@lucide/svelte/icons/badge-alert";
	import ShieldCheckIcon from "@lucide/svelte/icons/shield-check";
	import * as ButtonGroup from "$lib/components/ui/button-group/index.js";
	import { Button } from '$lib/components/ui/button/index.js';
	import { apiFetch } from '$lib/apiFetch';
	import { toast } from 'svelte-sonner';
	import QRCode from '@castlenine/svelte-qrcode';
	import * as AlertDialog from "$lib/components/ui/alert-dialog/index.js";
	import { buttonVariants } from '$lib/components/ui/button/index.js';
	import ArrowRightIcon from "@lucide/svelte/icons/arrow-right";
	import CopyIcon from "@lucide/svelte/icons/copy";
	import OtpForm2 from '$lib/components/otp-form2.svelte';
	import { getContext } from 'svelte';
	import type { IState } from '$lib/Abstractions/IState';
	import { Textarea } from "$lib/components/ui/textarea/index.js";

	const s = getContext<IState>('state');
	const isPaswordlessAccount = $derived(!s.hasPasswordConfigured);

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
			confirming2FaInProgress = false;
			showingRecoveryCodes = true;
		} else {
			toast.error("Invalid verification code.");
		}
	}
	
</script>

<div class="py-8">
	<div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
		<div class="flex items-start gap-3">
			<div class="flex size-9 shrink-0 items-center justify-center rounded-full bg-muted text-muted-foreground">
				<ShieldCheckIcon class="size-4" />
			</div>
			<div>
				<div class="flex flex-wrap items-center gap-2">
					<h2 class="text-base font-semibold tracking-tight">Two-factor authentication</h2>
					{#if twoFaEnabled}
						<Badge variant="secondary" class="bg-blue-500 text-white dark:bg-blue-600">
							<BadgeCheckIcon />
							Enabled
						</Badge>
					{:else if isPaswordlessAccount}
						<Badge variant="destructive">
							<BadgeAlertIcon />
							Disabled - No password set
						</Badge>
					{:else}
						<Badge variant="destructive">
							<BadgeAlertIcon />
							Disabled
						</Badge>
					{/if}
				</div>
				<p class="text-sm text-muted-foreground mt-1">Require a code from your authenticator app when signing in.</p>
			</div>
		</div>

		<ButtonGroup.Root class="ml-12 shrink-0 sm:ml-0">
			<AlertDialog.Root bind:open={qrCodeScanningInProgress}>
				<Button disabled={twoFaEnabled || isPaswordlessAccount} variant="outline" onclick={() => enable2FA()} class={buttonVariants({ variant: "outline" })}>
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
				<AlertDialog.Trigger disabled={!twoFaEnabled || isPaswordlessAccount} class={buttonVariants({ variant: "outline" })}>
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
	</div>

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
						<Textarea	value={recoveryCodes.join('\n')} readonly	class="w-full min-h-[200px] p-3 border rounded-md font-mono text-sm bg-muted"></Textarea>
						<Button type="button"	variant="default" class="w-full"
						        onclick={() => { navigator.clipboard.writeText(recoveryCodes.join('\n'));
								toast.success('Codes copied to clipboard');
							}}>
							<CopyIcon />
							Copy codes
						</Button>
					</div>
					<AlertDialog.Footer>			<AlertDialog.Cancel>Close</AlertDialog.Cancel>		</AlertDialog.Footer>	</AlertDialog.Content></AlertDialog.Root>

			<AlertDialog.Footer>
				<AlertDialog.Cancel>Close</AlertDialog.Cancel>
			</AlertDialog.Footer>
		</AlertDialog.Content>
	</AlertDialog.Root>
</div>