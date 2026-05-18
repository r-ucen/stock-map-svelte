<script lang="ts">
	import ManageAccount from '$lib/components/ManageAccount.svelte';

	let { data }: {
		data: {
			user: {
				email: string;
				isAuthenticated: boolean;
				roles: {
					admin: boolean;
					manager: boolean;
					customer: boolean;
				};
			};
		}
	} = $props();
</script>

<ManageAccount />