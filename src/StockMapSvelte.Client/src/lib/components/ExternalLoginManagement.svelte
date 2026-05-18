<script lang="ts">
	import { getContext } from 'svelte';
	import type { IState } from '$lib/Abstractions/IState';
	import { Card, CardContent, CardHeader, CardTitle } from '$lib/components/ui/card';
	import BadgeCheckIcon from '@lucide/svelte/icons/badge-check';
	import { Badge } from '$lib/components/ui/badge';
	import RemoveGoogleExternalLogin from '$lib/components/RemoveGoogleExternalLogin.svelte';

	const s = getContext<IState>('state');
	
	const noLoginConfigured = $derived(!s.hasExternalLoginConfigured);
	const hasPasswordConfigured = $derived(s.hasPasswordConfigured);
</script>

<Card>
	<CardHeader>
		<CardTitle>
			<span class="pr-2">Manage external logins</span>
			{#if noLoginConfigured}
				<Badge variant="destructive">
					<BadgeCheckIcon />
					No external login configured
				</Badge>
			{/if}
		</CardTitle>
	</CardHeader>
	<CardContent>
		{#if !noLoginConfigured}
			{#each s.externalLogins as item (item.loginProvider)}
				<Badge variant="secondary">
					{item.providerDisplayName}
					{#if (item.loginProvider === 'GoogleOpenIdConnect') && hasPasswordConfigured}
						<RemoveGoogleExternalLogin />
					{/if}
				</Badge>
			{/each}
		{/if}
	</CardContent>
</Card>