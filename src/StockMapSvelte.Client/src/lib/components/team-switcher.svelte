<script lang="ts">
	import * as DropdownMenu from "$lib/components/ui/dropdown-menu/index.js";
	import * as Sidebar from "$lib/components/ui/sidebar/index.js";
	import { useSidebar } from "$lib/components/ui/sidebar/index.js";
	import ChevronsUpDownIcon from "@lucide/svelte/icons/chevrons-up-down";
	import type { IState } from '$lib/Abstractions/IState';
	import { getContext } from 'svelte';

	const teams = getContext<IState>('state');
	const sidebar = useSidebar();

	let activeTeam = $derived(
		teams.portfolios.find(p => p.portfolioId === teams.selectedPortfolioId) ?? teams.portfolios[0]
	);
</script>

<Sidebar.Menu>
	<Sidebar.MenuItem>
		<DropdownMenu.Root>
			<DropdownMenu.Trigger>
				{#snippet child({ props })}
					<Sidebar.MenuButton
						{...props}
						size="lg"
						class="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
					>
						<div
							class="bg-sidebar-primary text-sidebar-primary-foreground flex aspect-square size-8 items-center justify-center rounded-lg"
						>
							<ChevronsUpDownIcon class="size-4" />
						</div>
						<div class="grid flex-1 text-start text-sm leading-tight">
							<span class="truncate font-medium">
								{activeTeam?.portfolioName ?? 'No Portfolios'}
							</span>
						</div>
						<ChevronsUpDownIcon class="ms-auto" />
					</Sidebar.MenuButton>
				{/snippet}
			</DropdownMenu.Trigger>
			<DropdownMenu.Content
				class="w-(--bits-dropdown-menu-anchor-width) min-w-56 rounded-lg"
				align="start"
				side={sidebar.isMobile ? "bottom" : "right"}
				sideOffset={4}
			>
				<DropdownMenu.Label class="text-muted-foreground text-xs">Portfolios</DropdownMenu.Label>
				{#each teams.portfolios as team, index (team.portfolioId)}
					<DropdownMenu.Item onSelect={() => (teams.selectedPortfolioId = team.portfolioId)} class="gap-2 p-2">
						<div class="flex size-6 items-center justify-center rounded-md border">
							<ChevronsUpDownIcon class="size-3.5 shrink-0" />
						</div>
						{team.portfolioName}
						<DropdownMenu.Shortcut>⌘{index + 1}</DropdownMenu.Shortcut>
					</DropdownMenu.Item>
				{/each}
			</DropdownMenu.Content>
		</DropdownMenu.Root>
	</Sidebar.MenuItem>
</Sidebar.Menu>
