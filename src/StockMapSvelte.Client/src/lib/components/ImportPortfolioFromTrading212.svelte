<script lang="ts">
	import { Button } from '$lib/components/ui/button/index';
	import * as Avatar from "$lib/components/ui/avatar/index.js";
	import Trading_212_PrimarySymbol from '$lib/assets/Trading_212_PrimarySymbol.svg';
	import { importPortfolioFromTrading212 } from '$lib/components/Portfolios/portfolioActions';
	import { toast } from 'svelte-sonner';
	import { getContext } from 'svelte';
	import type { IState } from '$lib/Abstractions/IState';
	import { buttonVariants } from '$lib/components/ui/button';
	import { Label } from '$lib/components/ui/label';
	import { Input } from '$lib/components/ui/input';
	import * as Dialog from "$lib/components/ui/dialog/index.js";
	import { Field, FieldGroup, FieldLabel } from '$lib/components/ui/field';
	import * as RadioGroup from "$lib/components/ui/radio-group/index.js";
	import { Spinner } from '$lib/components/ui/spinner';


	let s = getContext<IState>('state');
	
	let open = $state(false);
	
	let isBeingProcessed = $state(false);

	let isDemoAccount = $state("true");
	let portfolioNameBeingCreated = $state("");
	let trading212ApiKey = $state("");
	let trading212ApiSecret = $state("");
	
	function resetImportForm() {
		portfolioNameBeingCreated = "";
		isDemoAccount = "true";
		trading212ApiKey = "";
		trading212ApiSecret = "";
	}
	
	async function handleImportSubmit(e: SubmitEvent){
		e.preventDefault();
		isBeingProcessed = true;

		const isDemoAccountBoolean = isDemoAccount === "true";
		
		const result = await importPortfolioFromTrading212(s, isDemoAccountBoolean, portfolioNameBeingCreated, trading212ApiKey, trading212ApiSecret);
		if (!result.success) {
			toast.error(result.error ?? "An error occurred while importing portfolio");
		} else {
			resetImportForm();
			open = false;
			toast.success("Successfully imported portfolio");
		}
		isBeingProcessed = false;
	}
</script>

<Button
	variant="outline"
	size="sm"
	onclick={() => open = true}
>
	<Avatar.Root class="size-4 shrink-0">
		<Avatar.Image src={Trading_212_PrimarySymbol} alt="Trading 212 Logo" />
		<Avatar.Fallback class="text-[10px]">T</Avatar.Fallback>
	</Avatar.Root>
	Import Portfolio From Trading212
</Button>

<Dialog.Root bind:open>
	<Dialog.Content class="sm:max-w-[425px]" onInteractOutside={() => resetImportForm()}>
		<form onsubmit={handleImportSubmit}>
			<FieldGroup>
				
			<Dialog.Header class="mt-4">
				<Dialog.Title>Import your portfolio from Trading212</Dialog.Title>
				<Dialog.Description>
					Get or generate the API KEY ID and SECRET KEY in the settings of your 
					Trading212 account. Choose whether to import stocks from your demo or live account. 
					When creating the API key, enable the "Portfolio" option in permissions. 
					If you want to import stocks from the live account, 
					make sure to generate the API key when on the live version.
					Some stocks can fail to be imported, check the created portfolio and manually add them if possible.
				</Dialog.Description>
			</Dialog.Header>
			
				<Field>
					<FieldLabel for="name">Portfolio Name</FieldLabel>
					<Input
						id="name"
						required
						bind:value={portfolioNameBeingCreated}
						disabled={isBeingProcessed}
					/>
				</Field>
				
				<Field>
					<FieldLabel>Account Type</FieldLabel>
					<RadioGroup.Root bind:value={isDemoAccount} disabled={isBeingProcessed}>
						<div class="flex items-center space-x-2">
							<RadioGroup.Item value="true" id="demo-acc" />
							<Label for="demo-acc">Demo Account</Label>
						</div>
						<div class="flex items-center space-x-2">
							<RadioGroup.Item value="false" id="live-acc" />
							<Label for="live-acc">Live Account</Label>
						</div>
					</RadioGroup.Root>
				</Field>
				
				<Field>
					<FieldLabel for="key">API KEY ID</FieldLabel>
					<Input
						id="key"
						required
						bind:value={trading212ApiKey}
						disabled={isBeingProcessed}
					/>
				</Field>
				<Field>
					<FieldLabel for="secret">API SECRET</FieldLabel>
					<Input
						id="secret"
						required
						type="password"
						bind:value={trading212ApiSecret}
						disabled={isBeingProcessed}
					/>
				</Field>
			
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: "outline" })} onclick={resetImportForm}>
					Cancel
				</Dialog.Close>
				<Button
					type="submit"
					disabled={isBeingProcessed}
				>
					{#if isBeingProcessed}
						<Spinner />
						Processing...
					{:else}
						Save changes
					{/if}
				</Button>
			</Dialog.Footer>
			</FieldGroup>
		</form>
	</Dialog.Content>
</Dialog.Root>
