import type { IState } from '$lib/Abstractions/IState';
import { apiFetch } from '$lib/apiFetch';
import { toast } from 'svelte-sonner';

export async function fetchTreemapData(state: IState) {
	if (!state.selectedPortfolioId) {
		state.treemapData = null;
		return;
	}
	try {
		const response = await apiFetch(`/treemap-data/${state.selectedPortfolioId}`);
		if (!response.ok) {
			if (response.status === 429) {
				if (typeof window !== 'undefined') {
					toast.warning('Too many requests. Slow down.');
				}
			} else {
				if (typeof window !== 'undefined') {
					toast.error('Failed to fetch treemap data.');
				}
			}
			state.treemapData = null;
			return;
		}
		state.treemapData = await response.json();
		// eslint-disable-next-line @typescript-eslint/no-unused-vars
	} catch (error) {
		if (typeof window !== 'undefined') {
			toast.error('Error fetching treemap data');
		}
		state.treemapData = null;
	}
}
