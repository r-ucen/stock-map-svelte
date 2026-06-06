import { apiFetch } from '$lib/apiFetch';

export const load = async (event) => {
	const res = await apiFetch('/stocks/queried?PageNumber=1&PageSize=10', { event });

	if (!res.ok) {
		return {
			items: [],
			initialTotal: 0
		};
	}

	const result = await res.json();

	return {
		items: result.data,
		initialTotal: result.totalRecords
	};
};
