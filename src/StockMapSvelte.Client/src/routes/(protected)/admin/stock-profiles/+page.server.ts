import { apiFetch } from '$lib/apiFetch';

export const load = async (event) => {
	const res = await apiFetch('/stock-profiles?PageNumber=1&PageSize=10', { event });

	if (!res.ok) {
		return {
			stockProfiles: [],
			initialTotal: 0
		};
	}

	const result = await res.json();

	return {
		stockProfiles: result.data,
		initialTotal: result.totalRecords
	};
};
