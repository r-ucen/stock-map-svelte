import { createGuardHook } from 'svelte-guard';
import { sequence } from '@sveltejs/kit/hooks';
import { redirect, type Handle, error } from '@sveltejs/kit';
import { apiFetch } from '$lib/apiFetch';

const authHandle: Handle = async ({ event, resolve }) => {
	if (event.url.pathname === '/maintenance' || event.url.pathname === '/error') {
		return resolve(event);
	}

	try {
		const response = await apiFetch('/account/info', { event });

		if (response.status === 503) {
			throw redirect(307, '/maintenance');
		}

		if (response.status >= 500) {
			throw redirect(307, '/error');
		}

		const token = event.cookies.get('.AspNetCore.Identity.Application');

		if (response.ok && token) {
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
		if (err instanceof Error && err.message === 'VIEW_429') {
			throw error(429, 'Too many requests. Slow down and try again later.');
		}

		if (
			err &&
			typeof err === 'object' &&
			'status' in err &&
			(err.status === 307 || err.status === 302)
		) {
			throw err;
		}
		event.locals.user = undefined;
	}

	return resolve(event);
};

const guards = import.meta.glob('./routes/**/-guard.*');
const guardHandle = createGuardHook(guards);

export const handle = sequence(authHandle, guardHandle);
