import type { IStock } from '$lib/Abstractions/IStock';
import type { IStockProfile } from '$lib/Abstractions/IStockProfile';
import type { IPortfolio } from '$lib/Abstractions/IState';
import type { IUser } from '$lib/Abstractions/IUser';

export interface IAdminState {
	stocks: IStock[];
	stockProfiles: IStockProfile[];
	portfolios: IPortfolio[];
	users: IUser[];
}