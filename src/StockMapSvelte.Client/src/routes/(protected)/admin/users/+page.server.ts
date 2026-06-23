import { apiFetch } from '$lib/apiFetch';

export const load = async (event) => {
	const res = await apiFetch('/users?PageNumber=1&PageSize=10', { event });

	if (!res.ok) {
		return {
			users: [],
			initialTotal: 0
		};
	}

	const result = await res.json();

	return {
		users: result.data,
		initialTotal: result.totalRecords
	};
};
