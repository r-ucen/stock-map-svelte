<script>
	import { auth } from '$lib/auth.svelte';
	import LandingPage from '$lib/components/LandingPage.svelte';
	import Dashboard from '$lib/components/Dashboard.svelte';
	import { Spinner } from '$lib/components/ui/spinner';
	import { page } from '$app/state';
	import ManageAccount from '$lib/components/ManageAccount.svelte';
	import Portfolios from '$lib/components/Portfolios.svelte';
	import StockTreeMap from '$lib/components/StockTreeMap.svelte';
</script>

{#if !auth.isInitialized}
	<Spinner />
{:else if auth.isAuthenticated}
	<Dashboard>
		{#if page.url.searchParams.get('tab') === 'portfolios'}
			<Portfolios />
		{:else if page.url.searchParams.get('tab') === 'account'}
			<ManageAccount />
		{:else}
			<StockTreeMap />
		{/if}
	</Dashboard>
{:else}
	<LandingPage />
{/if}