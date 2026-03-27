import type { IStock } from '$lib/Abstractions/IStock';
import type { IStockProfile } from '$lib/Abstractions/IStockProfile';

export interface IAdminState {
	stocks: IStock[];
	stockProfiles: IStockProfile[];
}