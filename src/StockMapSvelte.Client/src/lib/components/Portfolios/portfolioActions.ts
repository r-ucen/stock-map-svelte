import { apiFetch } from '$lib/apiFetch';
import type { IPortfolio, IState } from '$lib/Abstractions/IState';

export async function setPortfolioAsDefault(id: string) {
	const res = await apiFetch(`/user-settings/default-portfolio`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json'
		},
		body: JSON.stringify({
			portfolioId: id
		})
	});

	if (res.ok) {
		return { success: true };
	} else {
		const errorData = await res.json().catch(() => ({}));
		const error = errorData.message || 'Failed to set portfolio as default';
		return { success: false, status: res.status, error };
	}
}

export async function deletePortfolio(state: IState, id: string) {
	const res = await apiFetch(`/portfolios/${id}`, {
		method: 'DELETE'
	});

	if (res.ok) {
		deletePortfolioInState(state, id);
		return { success: true };
	} else {
		const errorData = await res.json().catch(() => ({}));
		const error = errorData.message || 'Failed to delete portfolio';
		return { success: false, status: res.status, error };
	}
}

function deletePortfolioInState(state: IState, id: string) {
	state.portfolios = state.portfolios.filter((p) => p.portfolioId !== id);
}

function addPortfolioToState(state: IState, newPortfolio: IPortfolio) {
	state.portfolios = [...state.portfolios, newPortfolio];
}

export async function createPortfolio(state: IState, p_portfolioName: string, p_tickerSymbols: string[]) {
	const res = await apiFetch(`/portfolios`, {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json'
		},
		body: JSON.stringify({
			portfolioName: p_portfolioName,
			tickerSymbols: p_tickerSymbols
		})
	});

	if (res.ok) {
		const newPortfolio = await res.json();
		addPortfolioToState(state, newPortfolio);
		return { success: true };
	} else {
		const errorData = await res.json().catch(() => ({}));
		const error = errorData.message || 'Failed to create portfolio';
		return { success: false, status: res.status, error };
	}
}

export async function editPortfolio(state: IState, id: string, p_portfolioName: string, p_tickerSymbols: string[]) {
	const res = await apiFetch(`/portfolios/${id}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json'
		},
		body: JSON.stringify({
			portfolioName: p_portfolioName,
			tickerSymbols: p_tickerSymbols
		})
	});

	if (res.ok) {
		editPortfolioInState(state, id, p_portfolioName, p_tickerSymbols);
		return { success: true };
	} else {
		const errorData = await res.json().catch(() => ({}));
		const error = errorData.message || 'Failed to update portfolio';
		return { success: false, status: res.status, error };
	}
}

function editPortfolioInState(state: IState, id: string, p_portfolioName: string, p_tickerSymbols: string[]) {
	state.portfolios = state.portfolios.map((p) => {
		if (p.portfolioId === id) {
			return {
				...p,
				portfolioName: p_portfolioName,
				tickerSymbols: p_tickerSymbols
			};
		}
		return p;
	});
}

export function getPortfolioById(portfolios: IPortfolio[], id: string) : IPortfolio | undefined {
	return portfolios.find((p) => p.portfolioId === id);
}