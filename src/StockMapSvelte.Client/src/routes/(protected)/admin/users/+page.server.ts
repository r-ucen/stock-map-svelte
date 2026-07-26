import { apiFetch } from '$lib/apiFetch';

export const load = async (event) => {
	const pageNumber = Number(event.url.searchParams.get('PageNumber')) || 1;
	const pageSize = Number(event.url.searchParams.get('PageSize')) || 10;

	const res = await apiFetch(`/users?PageNumber=${pageNumber}&PageSize=${pageSize}`, { event });

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
