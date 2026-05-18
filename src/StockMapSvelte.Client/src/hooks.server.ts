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
		const [infoRes, accountInfo, adminRes, managerRes, customerRes] = await Promise.all([
			apiFetch('/manage/info', { event }),
			apiFetch('/account/info', { event }),
			apiFetch('/roles/is-admin', { event }),
			apiFetch('/roles/is-manager', { event }),
			apiFetch('/roles/is-customer', { event })
		]);

		if (infoRes.ok && accountInfo.ok) {
			const info = await infoRes.json();
			const aInfo = await accountInfo.json();

			event.locals.user = {
				email: info.email,
				isAuthenticated: true,
				roles: {
					admin: adminRes.status === 204,
					manager: managerRes.status === 204,
					customer: customerRes.status === 204
				},
				hasPasswordConfigured: aInfo.hasPasswordConfigured,
				hasExternalLoginConfigured: aInfo.hasExternalLoginConfigured,
				externalLogins: aInfo.externalLogins
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
