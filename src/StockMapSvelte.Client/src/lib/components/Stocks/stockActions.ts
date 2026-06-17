import { apiFetch } from '$lib/apiFetch';
import type { IStock } from '$lib/Abstractions/IStock';
import type { IAdminState } from '$lib/Abstractions/IAdminState';

export async function deleteStock(state: IAdminState, id: string) {
	const res = await apiFetch(`/stocks/${id}`, {
		method: 'DELETE'
	});

	if (res.ok) {
		deleteStockInState(state, id);
		return { success: true };
	} else {
		const errorData = await res.json().catch(() => ({}));
		const error = errorData.message || 'Failed to delete stock';
		return { success: false, status: res.status, error };
	}
}

function deleteStockInState(state: IAdminState, id: string) {
	state.stocks = state.stocks.filter((s) => s.id !== id);
}

function addStockToState(state: IAdminState, newStock: IStock) {
	state.stocks = [newStock, ...state.stocks].sort((a, b) =>
		a.tickerSymbol.localeCompare(b.tickerSymbol)
	);
}

export async function createMultipleStocks(state: IAdminState, p_tickerSymbols: string[]) {
	const res = await apiFetch(`/stocks/batch`, {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json'
		},
		body: JSON.stringify({
			tickerSymbols: p_tickerSymbols
		})
	});

	const data = await res.json().catch(() => null);

	console.log('API RESPONSE:', data);

	if (res.ok) {
		return {
			success: true,
			data
		};
	}

	return {
		success: false,
		status: res.status,
		data,
		error: data?.message ?? data?.failedToCreateStocks?.[0]?.error ?? 'Failed to create stocks'
	};
}

export async function createStock(state: IAdminState, p_tickerSymbol: string) {
	const res = await apiFetch(`/stocks`, {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json'
		},
		body: JSON.stringify({
			tickerSymbol: p_tickerSymbol
		})
	});

	if (res.ok) {
		const newStock = await res.json();
		console.log('Newly created stock from API:', newStock);
		// addStockToState(state, newStock);
		return { success: true };
	} else {
		const errorData = await res.json().catch(() => ({}));
		const error = errorData.message || 'Failed to create stock';
		return { success: false, status: res.status, error };
	}
}

export async function editStock(state: IAdminState, id: string, p_tickerSymbol: string) {
	const res = await apiFetch(`/stocks/${id}?ticker=${encodeURIComponent(p_tickerSymbol)}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json',
			Accept: '*/*'
		}
	});

	if (res.ok) {
		editStockInState(state, id, p_tickerSymbol);
		return { success: true };
	} else {
		const errorData = await res.json().catch(() => ({}));
		const error = errorData.message || 'Failed to update stock';
		return { success: false, status: res.status, error };
	}
}

function editStockInState(state: IAdminState, id: string, p_tickerSymbol: string) {
	state.stocks = state.stocks.map((s) => {
		if (s.id === id) {
			return {
				...s,
				tickerSymbol: p_tickerSymbol.toUpperCase()
			};
		}
		return s;
	});
}

export function getStockById(stocks: IStock[], id: string): IStock | undefined {
	return stocks.find((p) => p.id === id);
}
