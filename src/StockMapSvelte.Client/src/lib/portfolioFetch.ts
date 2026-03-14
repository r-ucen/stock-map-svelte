import type { IState } from '$lib/Abstractions/IState';
import { apiFetch } from '$lib/apiFetch';

export async function fetchTreemapData(state: IState) {
	if (!state.selectedPortfolioId) {
		state.treemapData = null;
		return;
	}
	try {
		const response = await apiFetch(`/treemap-data/${state.selectedPortfolioId}`);
		if (!response.ok) {
			throw new Error('Failed to fetch treemap data');
		}
		state.treemapData = await response.json();
	} catch (error) {
		console.error('Error fetching treemap data:', error);
		state.treemapData = null;
	}
}
