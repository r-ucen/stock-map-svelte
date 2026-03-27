import type { IStock } from '$lib/Abstractions/IStock';

export interface IAdminState {
	stocks: IStock[]
}