<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import SunIcon from '@lucide/svelte/icons/sun';
	import MoonIcon from '@lucide/svelte/icons/moon';
	import { toggleMode } from "mode-watcher";
</script>

<header class="sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
	<div class="container mx-auto px-4 md:px-8 h-16 flex items-center justify-between">
		<a href="/" class="flex items-center gap-2 transition-opacity hover:opacity-80">

			<span class="text-muted-foreground font-bold text-xl tracking-tight">Equmap</span>
		</a>

		<nav class="flex items-center gap-4">
			<Button onclick={toggleMode} variant="ghost" size="icon" class="relative">
				<SunIcon class="h-[1.2rem] w-[1.2rem] transition-all scale-100 rotate-0 dark:-rotate-90 dark:scale-0" />
				<MoonIcon class="absolute h-[1.2rem] w-[1.2rem] transition-all scale-0 rotate-90 dark:rotate-0 dark:scale-100" />
				<span class="sr-only">Toggle theme</span>
			</Button>
		</nav>
	</div>
</header>
