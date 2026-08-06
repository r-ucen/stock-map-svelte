import type { Guard } from 'svelte-guard';
import { redirect } from '@sveltejs/kit';

export const guard: Guard = async ({ locals }) => {
	if (locals.isBanned) {
		throw redirect(307, '/banned');
	}
	if (!locals.user) {
		throw redirect(307, '/login');
	}
	return true;
};
