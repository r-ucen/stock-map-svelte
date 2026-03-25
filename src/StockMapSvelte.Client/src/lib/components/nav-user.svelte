<script lang="ts">
	import * as Avatar from "$lib/components/ui/avatar/index.js";
	import * as DropdownMenu from "$lib/components/ui/dropdown-menu/index.js";
	import * as Sidebar from "$lib/components/ui/sidebar/index.js";
	import { useSidebar } from "$lib/components/ui/sidebar/index.js";
	import BadgeCheckIcon from "@lucide/svelte/icons/badge-check";
	import BellIcon from "@lucide/svelte/icons/bell";
	import ChevronsUpDownIcon from "@lucide/svelte/icons/chevrons-up-down";
	import CreditCardIcon from "@lucide/svelte/icons/credit-card";
	import LogOutIcon from "@lucide/svelte/icons/log-out";
	import SparklesIcon from "@lucide/svelte/icons/sparkles";
	import { toggleMode } from "mode-watcher";
	import SunIcon from "@lucide/svelte/icons/sun";
	import MoonIcon from "@lucide/svelte/icons/moon";
	import { Button } from "$lib/components/ui/button/index.js";
	import { auth } from "$lib/auth.svelte.js";

	let { userEmail }: { userEmail: string } = $props();

	let userEmailFirstLetter = $derived(userEmail.charAt(0).toUpperCase());
	let userEmailName = $derived(userEmail.split("@")[0]);
	
	const sidebar = useSidebar();

</script>

<Sidebar.Menu>
	<Sidebar.MenuItem>
		<DropdownMenu.Root>
			<DropdownMenu.Trigger>
				{#snippet child({ props })}
					<Sidebar.MenuButton
						size="lg"
						class="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
						{...props}
					>
						<Avatar.Root class="size-8 rounded-lg">
							<Avatar.Fallback class="rounded-lg">{userEmailFirstLetter}</Avatar.Fallback>
						</Avatar.Root>
						<div class="grid flex-1 text-start text-sm leading-tight">
							<span class="truncate font-medium">{userEmailName}</span>
							<span class="truncate text-xs">{userEmail}</span>
						</div>
						<ChevronsUpDownIcon class="ms-auto size-4" />
					</Sidebar.MenuButton>
				{/snippet}
			</DropdownMenu.Trigger>
			<DropdownMenu.Content
				class="w-(--bits-dropdown-menu-anchor-width) min-w-56 rounded-lg"
				side={sidebar.isMobile ? "bottom" : "right"}
				align="end"
				sideOffset={4}
			>
				<DropdownMenu.Label class="p-0 font-normal">
					<div class="flex items-center gap-2 px-1 py-1.5 text-start text-sm">
						<Avatar.Root class="size-8 rounded-lg">
							<Avatar.Fallback class="rounded-lg">{userEmailFirstLetter}</Avatar.Fallback>
						</Avatar.Root>
						<div class="grid flex-1 text-start text-sm leading-tight">
							<span class="truncate font-medium">{userEmailName}</span>
							<span class="truncate text-xs">{userEmail}</span>
						</div>
						<Button onclick={toggleMode} variant="outline" size="icon">
							<SunIcon
								class="h-[1.2rem] w-[1.2rem] scale-100 rotate-0 !transition-all dark:scale-0 dark:-rotate-90"
							/>
							<MoonIcon
								class="absolute h-[1.2rem] w-[1.2rem] scale-0 rotate-90 !transition-all dark:scale-100 dark:rotate-0"
							/>
							<span class="sr-only">Toggle theme</span>
						</Button>
					</div>
				</DropdownMenu.Label>
				<DropdownMenu.Group>
					<DropdownMenu.Item>
						<BadgeCheckIcon />
						<a href="/?tab=account">Account</a>
					</DropdownMenu.Item>
				</DropdownMenu.Group>
				<DropdownMenu.Separator />
				<DropdownMenu.Item onSelect={() => auth.logout()}>
					<LogOutIcon />
					Log out
				</DropdownMenu.Item>
			</DropdownMenu.Content>
		</DropdownMenu.Root>
	</Sidebar.MenuItem>
</Sidebar.Menu>
