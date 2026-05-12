import { apiFetch } from '$lib/apiFetch';

export const load = async (event) => {
	const res = await apiFetch('/portfolios/me', { event });

	if (!res.ok) {
		return {
			user: event.locals.user,
			portfolios: []
		};
	}

	if (res.status === 204) {
		return {
			user: event.locals.user,
			portfolios: []
		};
	}

	try {
		const portfolios = await res.json();
		return {
			user: event.locals.user,
			portfolios
		};
	} catch (err) {
		return {
			user: event.locals.user,
			portfolios: []
		};
	}
};
