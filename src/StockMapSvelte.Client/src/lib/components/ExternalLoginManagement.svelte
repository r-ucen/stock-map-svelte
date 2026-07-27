<script lang="ts">
	import { getContext } from 'svelte';
	import type { IState } from '$lib/Abstractions/IState';
	import BadgeAlertIcon from '@lucide/svelte/icons/badge-alert';
	import Link2Icon from '@lucide/svelte/icons/link-2';
	import { Badge } from '$lib/components/ui/badge';
	import RemoveGoogleExternalLogin from '$lib/components/RemoveGoogleExternalLogin.svelte';
	import * as Item from "$lib/components/ui/item/index.js";
	const s = getContext<IState>('state');
	
	const noLoginConfigured = $derived(!s.hasExternalLoginConfigured);
	const hasPasswordConfigured = $derived(s.hasPasswordConfigured);
</script>

<div class="py-8">
	<div class="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
		<div class="flex items-start gap-3">
			<div class="flex size-9 shrink-0 items-center justify-center rounded-full bg-muted text-muted-foreground">
				<Link2Icon class="size-4" />
			</div>
			<div>
				<div class="flex flex-wrap items-center gap-2">
					<h2 class="text-base font-semibold tracking-tight">External logins</h2>
					{#if noLoginConfigured}
						<Badge variant="destructive">
							<BadgeAlertIcon />
							No external login configured
						</Badge>
					{/if}
				</div>
				<p class="text-sm text-muted-foreground mt-1">Accounts you can use to sign in instead of a password.</p>
			</div>
		</div>

		{#if !noLoginConfigured}
			<div class="ml-12 flex w-full max-w-md flex-col gap-6">
				{#each s.externalLogins as item (item.loginProvider)}
					<div class="mr-3 sm:mr-0">
						<Item.Root variant="outline" size="sm">
							<Item.Content>
								<Item.Title>{item.providerDisplayName}</Item.Title>
								<Item.Description
								>{item.loginProvider}</Item.Description
								>
							</Item.Content>
							<Item.Actions>
								{#if (item.loginProvider === 'GoogleOpenIdConnect') && hasPasswordConfigured}
									<RemoveGoogleExternalLogin />
								{/if}
							</Item.Actions>
						</Item.Root>
					</div>
				{/each}
			</div>
		{/if}
	</div>
</div>