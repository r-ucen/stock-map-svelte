<script lang="ts">
	import { page } from '$app/state';
</script>

<div class="flex min-h-screen items-center justify-center bg-background px-4">
	<div class="text-center">
		<h1 class="text-primary leading-tighter mb-6 text-4xl font-semibold tracking-tight text-balance lg:leading-[1.1] lg:font-semibold xl:text-5xl xl:tracking-tighter max-w-4xl">{page.status} {page.error?.message}</h1>
	</div>
</div>