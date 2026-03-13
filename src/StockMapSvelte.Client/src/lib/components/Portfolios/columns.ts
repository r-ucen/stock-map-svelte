import type { ColumnDef } from '@tanstack/table-core';
import type { IPortfolio} from '$lib/Abstractions/IState';

export const columns: ColumnDef<IPortfolio>[] = [
	{
		accessorKey: 'portfolioName',
		header: 'Name'
	},
	{
		accessorKey: 'isDefault',
		header: 'Is Default',
		cell: ({ getValue }) => {
			const isDefault = getValue() as boolean;
			return isDefault ? 'Yes' : 'No';
		}
	},
	{
		accessorKey: 'tickerSymbols',
		header: 'Ticker Symbols',
		cell: ({ getValue }) => {
			const tickerSymbols = getValue() as string[];
			return tickerSymbols.slice(0, 4).join(', ');
		}
	}
];