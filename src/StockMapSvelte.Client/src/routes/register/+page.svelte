<script>
	import GalleryVerticalEndIcon from "@lucide/svelte/icons/gallery-vertical-end";
	import SignupForm from "$lib/components/signup-form.svelte";
	import { resolve } from '$app/paths';
	
	const resolvedRoot = resolve('/');
</script>

<div class="bg-muted flex min-h-svh flex-col items-center justify-center gap-6 p-6 md:p-10">
	<div class="flex w-full max-w-sm flex-col gap-6">
		<a href={resolvedRoot} class="flex items-center gap-2 self-center font-medium">
			<div
				class="bg-primary text-primary-foreground flex size-6 items-center justify-center rounded-md"
			>
				<GalleryVerticalEndIcon class="size-4" />
			</div>
			Equmap
		</a>
		<SignupForm />
	</div>
</div>
