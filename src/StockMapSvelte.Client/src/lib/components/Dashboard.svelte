<script lang="ts">
	import AppSidebar from "$lib/components/app-sidebar.svelte";
	import * as Breadcrumb from "$lib/components/ui/breadcrumb/index.js";
	import { Separator } from "$lib/components/ui/separator/index.js";
	import * as Sidebar from "$lib/components/ui/sidebar/index.js";

	let { children } = $props();

	import { page } from "$app/state";
	import { onMount, setContext } from 'svelte';
	import GalleryVerticalEndIcon from '@lucide/svelte/icons/gallery-vertical-end';
	import type { IPortfolio, IState } from '$lib/Abstractions/IState';
	import { apiFetch } from '$lib/apiFetch';
	
	const tabNames: Record<string, string> = {
		'portfolios': 'Portfolios',
		'account': 'Manage Account',
	};

	let currentTabName = $derived(
		tabNames[page.url.searchParams.get('tab') ?? ''] ?? 'Stock Map'
	);
	
	let state = $state<IState>({
		portfolios: [],
		portfolioLogoDefault: GalleryVerticalEndIcon,
		selectedPortfolioId: null,
	})
	setContext('state', state);
	
	onMount(async () => {
		try {
			const response = await apiFetch('/portfolios/me');
			if (!response.ok) {
				throw new Error('Failed to fetch portfolios');
			}
			const data: IPortfolio[] = await response.json();
			state.portfolios = data;
			state.selectedPortfolioId = data.filter(p => p.isDefault)[0]?.portfolioId ?? null;
		} catch (error) {
			console.error('Error fetching portfolios:', error);
		}
	})
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
		<div class="flex flex-1 flex-col gap-4 p-4 pt-0">
			{@render children()}
		</div>
	</Sidebar.Inset>
</Sidebar.Provider>
