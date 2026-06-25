import { apiFetch } from '$lib/apiFetch';

export const load = async (event) => {
	const res = await apiFetch('/portfolios?PageNumber=1&PageSize=10', { event });

	if (!res.ok) {
		return {
			portfolios: [],
			initialTotal: 0
		};
	}

	const result = await res.json();

	return {
		portfolios: result.data,
		initialTotal: result.totalRecords
	};
};
