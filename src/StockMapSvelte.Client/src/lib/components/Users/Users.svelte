<script lang="ts">
	import DataTable from './data-table.svelte'
	import { columns } from "./columns.js";
	import { getContext } from 'svelte';
	import type { IAdminState } from '$lib/Abstractions/IAdminState';
	import type { IUser } from '$lib/Abstractions/IUser';
	import { apiFetch } from '$lib/apiFetch';
	import { toast } from 'svelte-sonner';
	import PaginationControls from '$lib/components/PaginationControls.svelte';

	let s = getContext<IAdminState>('stateAdmin');
	let { items, initialTotal }: {items: IUser[], initialTotal: number} = $props()

	$effect(() => {
		if (s.users.length === 0 && items?.length > 0) {
			s.users = items;
		}
	});

	let pageNumber = $state(1);
	let pageSize = $state(10);
	let totalRecords = $derived(initialTotal || 0);

	$effect(() => {
		// eslint-disable-next-line @typescript-eslint/no-unused-expressions
		s.refreshUsers;
		async function fetchPagedUsers() {
			try {
				const params = new URLSearchParams({
					PageNumber: pageNumber.toString(),
					PageSize: pageSize.toString()
				});

				const res = await apiFetch(`/users?${params.toString()}`);

				if (res.ok) {
					const result = await res.json();
					s.users = result.data;
					totalRecords = result.totalRecords;
				} else {
					toast.error("Failed to fetch users");
				}
			} catch (error) {
				console.error("Error fetching users:", error);
			}
		}

		fetchPagedUsers();
	});

</script>

<DataTable data={s.users} {columns}  />

<PaginationControls
	count={totalRecords}
	perPage={pageSize}
	bind:page={pageNumber}
/>