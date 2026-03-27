import type { Guard } from 'svelte-guard';
import { redirect } from '@sveltejs/kit';

export const guard: Guard = async ({ locals }) => {
	if (!locals.user?.roles.manager) {
		throw redirect(307, '/map');
	}
	return true;
};
