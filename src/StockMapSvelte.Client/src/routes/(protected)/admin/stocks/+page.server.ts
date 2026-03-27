import { apiFetch } from '$lib/apiFetch';

export const load = async (event) => {
	const res = await apiFetch('/stocks', { event });
	
	return {
		stocks: await res.json()
	};
};
