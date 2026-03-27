import { apiFetch } from '$lib/apiFetch';

export const load = async (event) => {
	const res = await apiFetch('/users', { event });
	
	if (!res.ok) {
		return {
			users: []
		};
	}

	return {
		users: await res.json()
	};
};

