<script lang="ts">
	import type { IUser } from '$lib/Abstractions/IUser';
	import Users from '$lib/components/Users/Users.svelte';

	let { data }: {
		data: {
			users: IUser[];
			initialTotal: number;
		};
	} = $props();
</script>

<Users items={data.users} initialTotal={data.initialTotal}/>

