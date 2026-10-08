<script lang="ts" module>
	import ChartPieIcon from "@lucide/svelte/icons/chart-pie";
	import MapIcon from "@lucide/svelte/icons/map";
	import ShieldIcon from "@lucide/svelte/icons/shield";

	const data = {
		navMain: [
			{
				title: "Portfolios",
				url: "/portfolios",
				icon: ChartPieIcon,
				isActive: true,
				isAdminOnly: false,
				items: [],
			},
			{
				title: "Stock Map ",
				url: "/map",
				icon: MapIcon,
				isAdminOnly: false,
				items: [],
			},
			{
				title: "Administration",
				url: "/#",
				icon: ShieldIcon,
				isAdminOnly: true,
				items: [
					{
						title: "Stocks",
						url: "/admin/stocks",
					},
					{
						title: "Stock Profiles",
						url: "/admin/stock-profiles"
					},
					{
						title: "Portfolios",
						url: "/admin/portfolios"
					},
					{
						title: "Users",
						url: "/admin/users"
					}
				],
			}
		],
	};
</script>

<script lang="ts">
	import NavMain from "./nav-main.svelte";
	import NavUser from "./nav-user.svelte";
	import TeamSwitcher from "./team-switcher.svelte";
	import * as Sidebar from "$lib/components/ui/sidebar/index.js";
	import { type ComponentProps, getContext } from 'svelte';
	import MapSettings from "$lib/components/map-settings.svelte";
	import { useSidebar } from "$lib/components/ui/sidebar/index.js";
	import type { IState } from '$lib/Abstractions/IState';
	import { page } from '$app/state';
	import Legend from '$lib/components/Legend.svelte';
	import LastUpdated from '$lib/components/LastUpdated.svelte';

	let {
		ref = $bindable(null),
		collapsible = "icon",
		...restProps
	}: ComponentProps<typeof Sidebar.Root> = $props();
	
	const sidebar = useSidebar();

	let currentPathName = $derived(
		page.url.pathname ?? 'Stock Map'
	);

	'/map'
	
	let s = getContext<IState>('state');
</script>

<Sidebar.Root {collapsible} {...restProps}>
	<Sidebar.Header>
		<TeamSwitcher />
	</Sidebar.Header>
	<Sidebar.Content>
		<NavMain items={data.navMain} data={page.data}/>

		{#if sidebar.state !== "collapsed" && currentPathName === '/map'}
			<MapSettings />
			<Legend selectedMetric={s.selectedMetric} />
			<LastUpdated />
		{/if}
		
	</Sidebar.Content>
	<Sidebar.Footer>
		<NavUser userEmail={s.email ?? ""} />
	</Sidebar.Footer>
	<Sidebar.Rail />
</Sidebar.Root>
