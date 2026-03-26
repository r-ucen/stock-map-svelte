import { apiFetch } from '$lib/apiFetch';

export const load = async (event) => {
	const res = await apiFetch('/portfolios/me', { event });
	
	return {
		user: event.locals.user,
		portfolios: await res.json()
	};
};