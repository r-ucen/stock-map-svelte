<script lang="ts" module>
	import AudioWaveformIcon from "@lucide/svelte/icons/audio-waveform";
	import ChartPieIcon from "@lucide/svelte/icons/chart-pie";
	import CommandIcon from "@lucide/svelte/icons/command";
	import GalleryVerticalEndIcon from "@lucide/svelte/icons/gallery-vertical-end";
	import MapIcon from "@lucide/svelte/icons/map";

	const data = {
		user: {
			name: "shadcn",
			email: "m@example.com",
			avatar: "/avatars/shadcn.jpg",
		},
		portfolios: [
			{
				name: "Acme Inc",
				logo: GalleryVerticalEndIcon,
				plan: "Enterprise",
			},
			{
				name: "Acme Corp.",
				logo: AudioWaveformIcon,
				plan: "Startup",
			},
			{
				name: "Evil Corp.",
				logo: CommandIcon,
				plan: "Free",
			},
		],
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

	let {
		ref = $bindable(null),
		collapsible = "icon",
		...restProps
	}: ComponentProps<typeof Sidebar.Root> = $props();
</script>

<Sidebar.Root {collapsible} {...restProps}>
	<Sidebar.Header>
		<TeamSwitcher teams={data.portfolios} />
	</Sidebar.Header>
	<Sidebar.Content>
		<NavMain items={data.navMain} />
	</Sidebar.Content>
	<Sidebar.Footer>
		<NavUser user={data.user} />
	</Sidebar.Footer>
	<Sidebar.Rail />
</Sidebar.Root>
