import { apiFetch } from '$lib/apiFetch';

export const load = async (event) => {
	const pageNumber = Number(event.url.searchParams.get('PageNumber')) || 1;
	const pageSize = Number(event.url.searchParams.get('PageSize')) || 10;
	
	const res = await apiFetch(`/portfolios?PageNumber=${pageNumber}&PageSize=${pageSize}`, {
		event
	});

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
