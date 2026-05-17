<script lang="ts">
	import AppSidebar from "$lib/components/app-sidebar.svelte";
	import * as Breadcrumb from "$lib/components/ui/breadcrumb/index.js";
	import { Separator } from "$lib/components/ui/separator/index.js";
	import * as Sidebar from "$lib/components/ui/sidebar/index.js";

	import { page } from "$app/state";
	import { onDestroy, onMount, setContext } from 'svelte';
	import GalleryVerticalEndIcon from '@lucide/svelte/icons/gallery-vertical-end';
	import { type IPortfolio, type IState, MapMetric } from '$lib/Abstractions/IState';
	import { fetchTreemapData } from '$lib/portfolioFetch';
	import { apiFetch } from '$lib/apiFetch';

	type ProtectedLayoutData = {
		user?: {
			email: string;
			isAuthenticated: boolean;
			roles: {
				admin: boolean;
				manager: boolean;
				customer: boolean;
			};
		};
		portfolios: IPortfolio[];
	};

	let { data, children }: { data: ProtectedLayoutData; children: import('svelte').Snippet } = $props();

	const pageTitles: Record<string, string> = {
		'/portfolios': 'Portfolios',
		'/account': 'Manage Account',
		'/map': 'Stock Map',
		'/admin/stocks': 'Manage Stocks',
		'/admin/stock-profiles': 'Stock Profiles',
		'/admin/portfolios': 'Manage Portfolios',
		'/admin/users': 'Manage Users',
	};

	let currentTabName = $derived(
		pageTitles[page.url.pathname] ?? 'Stock Map'
	);

	let s = $state<IState>({
		email: undefined,
		portfolios: [],
		portfolioLogoDefault: GalleryVerticalEndIcon,
		selectedPortfolioId: null,
		selectedMetric: null,
		treemapData: null,
	})
	setContext('state', s);

	$effect(() => {
		s.email = data.user?.email;
		s.portfolios = data.portfolios;
		s.selectedPortfolioId = data.portfolios.filter(p => p.isDefault)[0]?.portfolioId ?? null;
		s.selectedMetric = MapMetric.RegularMarketChangePercent;
	});
	
	onMount(async () => {
		if (page.url.searchParams.get('login') === 'true') {
			const res = await apiFetch('/portfolios/me');
			if (res.ok) {
				const updatedPortfolios = await res.json();

				s.portfolios = updatedPortfolios;
				if (updatedPortfolios.length > 0) {
					s.selectedPortfolioId = updatedPortfolios.find(p => p.isDefault)?.portfolioId ?? updatedPortfolios[0].portfolioId;
				}
			}

			const newUrl = new URL(window.location.href);
			newUrl.searchParams.delete('login');
			window.history.replaceState({}, '', newUrl);
		}
	});

</script>


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
