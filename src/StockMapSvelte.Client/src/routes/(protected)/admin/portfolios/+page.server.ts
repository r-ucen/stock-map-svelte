import { apiFetch } from '$lib/apiFetch';

export const load = async (event) => {
	const res = await apiFetch('/portfolios', { event });
	
	if (!res.ok) {
		return {
			portfolios: []
		};
	}

	return {
		portfolios: await res.json()
	};
};

