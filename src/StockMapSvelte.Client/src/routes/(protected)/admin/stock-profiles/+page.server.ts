import { apiFetch } from '$lib/apiFetch';

export const load = async (event) => {
	const res = await apiFetch('/stock-profiles', { event });
	
	if (!res.ok) {
		return {
			stockProfiles: []
		};
	}

	return {
		stockProfiles: await res.json()
	};
};

