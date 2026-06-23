import { apiFetch } from '$lib/apiFetch';

export const load = async (event) => {
	const res = await apiFetch('/stocks?PageNumber=1&PageSize=10', { event });

	if (!res.ok) {
		return {
			stocks: [],
			initialTotal: 0
		};
	}

	const result = await res.json();

	return {
		stocks: result.data,
		initialTotal: result.totalRecords
	};
};
