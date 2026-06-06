<script lang="ts">
	import * as Pagination from "$lib/components/ui/pagination/index.js";

	let {
		count,
		perPage = 10,
		page = $bindable()
	}: {
		count: number;
		perPage?: number;
		page: number;
	} = $props();
</script>

<div class="flex items-center justify-center space-x-2 py-4">
	<Pagination.Root {count} {perPage} bind:page>
		{#snippet children({ pages, currentPage })}
			<Pagination.Content>
				<Pagination.Item>
					<Pagination.Previous />
				</Pagination.Item>
				{#each pages as page (page.key)}
					{#if page.type === "ellipsis"}
						<Pagination.Item>
							<Pagination.Ellipsis />
						</Pagination.Item>
					{:else}
						<Pagination.Item>
							<Pagination.Link {page} isActive={currentPage === page.value}>
								{page.value}
							</Pagination.Link>
						</Pagination.Item>
					{/if}
				{/each}
				<Pagination.Item>
					<Pagination.Next />
				</Pagination.Item>
			</Pagination.Content>
		{/snippet}
	</Pagination.Root>
</div>