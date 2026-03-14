<script lang="ts" module>
	import ChartPieIcon from "@lucide/svelte/icons/chart-pie";
	import MapIcon from "@lucide/svelte/icons/map";

	const data = {
		user: {
			name: "shadcn",
			email: "m@example.com",
			avatar: "/avatars/shadcn.jpg",
		},
		navMain: [
			{
				title: "Portfolios",
				url: "/?tab=portfolios",
				icon: ChartPieIcon,
				isActive: true,
				items: [],
			},
			{
				title: "Stock Map ",
				url: "/",
				icon: MapIcon,
				items: [],
			}
		],
	};
</script>

<script lang="ts">
	import NavMain from "./nav-main.svelte";
	import NavUser from "./nav-user.svelte";
	import TeamSwitcher from "./team-switcher.svelte";
	import * as Sidebar from "$lib/components/ui/sidebar/index.js";
	import type { ComponentProps } from "svelte";
	import MapSettings from "$lib/components/map-settings.svelte";
	import { useSidebar } from "$lib/components/ui/sidebar/index.js";

	let {
		ref = $bindable(null),
		collapsible = "icon",
		...restProps
	}: ComponentProps<typeof Sidebar.Root> = $props();
	
	const sidebar = useSidebar();

</script>

<Sidebar.Root {collapsible} {...restProps}>
	<Sidebar.Header>
		<TeamSwitcher />
	</Sidebar.Header>
	<Sidebar.Content>
		<NavMain items={data.navMain} />

		{#if sidebar.state !== "collapsed"}
			<MapSettings />
		{/if}
		
	</Sidebar.Content>
	<Sidebar.Footer>
		<NavUser user={data.user} />
	</Sidebar.Footer>
	<Sidebar.Rail />
</Sidebar.Root>
