import { apiFetch } from '$lib/apiFetch';
import type { IPortfolio, IState } from '$lib/Abstractions/IState';


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
	
	console.log(res);

	if (res.ok) {
		console.log(res.body);
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