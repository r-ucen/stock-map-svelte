import { createGuardHook } from 'svelte-guard';
import { sequence } from '@sveltejs/kit/hooks';
import type { Handle } from '@sveltejs/kit';
import { apiFetch } from '$lib/apiFetch';

const authHandle: Handle = async ({ event, resolve }) => {
	const token = event.cookies.get('.AspNetCore.Identity.Application');

	if (!token) {
		event.locals.user = undefined;
		return resolve(event);
	}

	try {
		const response = await apiFetch('/account/info', { event });

		if (response.ok) {
			const data = await response.json();

			event.locals.user = {
				email: data.email,
				isAuthenticated: true,
				roles: {
					admin: data.isAdmin,
					manager: data.isManager,
					customer: data.isCustomer
				},
				hasPasswordConfigured: data.hasPasswordConfigured,
				hasExternalLoginConfigured: data.hasExternalLoginConfigured,
				externalLogins: data.externalLogins
			};
		} else {
			event.locals.user = undefined;
		}
	} catch (err) {
		console.error('Auth check failed:', err);
		event.locals.user = undefined;
	}

	return resolve(event);
};

const guards = import.meta.glob('./routes/**/-guard.*');
const guardHandle = createGuardHook(guards);

export const handle = sequence(authHandle, guardHandle);
