<script lang="ts">
	import AppSidebar from "$lib/components/app-sidebar.svelte";
	import * as Breadcrumb from "$lib/components/ui/breadcrumb/index.js";
	import { Separator } from "$lib/components/ui/separator/index.js";
	import * as Sidebar from "$lib/components/ui/sidebar/index.js";

	let { children } = $props();

	import { page } from "$app/state";
	import { onDestroy, onMount, setContext } from 'svelte';
	import GalleryVerticalEndIcon from '@lucide/svelte/icons/gallery-vertical-end';
	import { type IPortfolio, type IState, MapMetric } from '$lib/Abstractions/IState';
	import { apiFetch } from '$lib/apiFetch';
	import { fetchTreemapData } from '$lib/portfolioFetch';
	import { auth } from '$lib/auth.svelte';
	import { goto } from '$app/navigation';
	import { Spinner } from '$lib/components/ui/spinner';

	const pageTitles: Record<string, string> = {
		'/portfolios': 'Portfolios',
		'/account': 'Manage Account',
		'/map': 'Stock Map',
	};

	let currentTabName = $derived(
		pageTitles[page.url.pathname] ?? 'Stock Map'
	);

	let state = $state<IState>({
		email: null,
		portfolios: [],
		portfolioLogoDefault: GalleryVerticalEndIcon,
		selectedPortfolioId: null,
		selectedMetric: null,
		treemapData: null,
	})
	setContext('state', state);

	$effect(() => {
		if (auth.isInitialized && !auth.isAuthenticated) {
			goto('/login');
		}
	});

	$effect(() => {
		if (auth.user) {
			state.email = auth.user.email;
		}
	});

	let treemapInterval: ReturnType<typeof setInterval> | undefined;

	$effect(() => {
		clearInterval(treemapInterval);
		if (state.selectedPortfolioId) {
			fetchTreemapData(state);
			treemapInterval = setInterval(() => fetchTreemapData(state), 30000);
		}
	});

	onDestroy(() => {
		clearInterval(treemapInterval);
	});

	$effect(() => {
		let interval: ReturnType<typeof setInterval>;

		if (state.selectedPortfolioId) {
			fetchTreemapData(state);
			interval = setInterval(() => fetchTreemapData(state), 30000);
		}

		return () => {
			if (interval) clearInterval(interval);
		};
	});

	onMount(async () => {
		try {
			const portfoliosResponse = await apiFetch('/portfolios/me');
			if (!portfoliosResponse.ok) {
				throw new Error('Failed to fetch portfolios');
			}

			state.email = auth?.user?.email ?? null;

			const portfolios: IPortfolio[] = await portfoliosResponse.json();
			state.portfolios = portfolios;
			state.selectedPortfolioId = portfolios.filter(p => p.isDefault)[0]?.portfolioId ?? null;
			state.selectedMetric = MapMetric.RegularMarketChangePercent;
		} catch (error) {
			console.error('Error fetching data:', error);
		}
	})
</script>

{#if !auth.isInitialized}
	<div class="h-screen w-full flex items-center justify-center">
		<Spinner />
	</div>
{:else}
	<Sidebar.Provider>
		<AppSidebar />
		<Sidebar.Inset>
			<header
				class="flex h-16 shrink-0 items-center gap-2 transition-[width,height] ease-linear group-has-data-[collapsible=icon]/sidebar-wrapper:h-12"
			>
				<div class="flex items-center gap-2 px-4">
					<Sidebar.Trigger class="-ms-1" />
					<Separator orientation="vertical" class="me-2 data-[orientation=vertical]:h-4" />
					<Breadcrumb.Root>
						<Breadcrumb.List>
							<Breadcrumb.Item>
								<Breadcrumb.Page>{currentTabName}</Breadcrumb.Page>
							</Breadcrumb.Item>
						</Breadcrumb.List>
					</Breadcrumb.Root>
				</div>
			</header>
			{#if currentTabName === 'Stock Map'}
				<div class="relative flex flex-1 flex-col overflow-hidden min-h-0 min-w-0">
					{@render children()}
				</div>
			{:else}
				<div class="flex flex-1 flex-col gap-4 p-4 pt-0">
					{@render children()}
				</div>
			{/if}
		</Sidebar.Inset>
	</Sidebar.Provider>
{/if}
