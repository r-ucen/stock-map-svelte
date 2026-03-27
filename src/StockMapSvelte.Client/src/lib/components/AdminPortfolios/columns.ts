import type { ColumnDef } from '@tanstack/table-core';
import { renderComponent } from '$lib/components/ui/data-table';
import DataTableNameButton from '$lib/components/AdminPortfolios/data-table-name-button.svelte';
import type { IPortfolio } from '$lib/Abstractions/IState';

export const columns: ColumnDef<IPortfolio>[] = [
	{
		accessorKey: 'portfolioName',
		header: ({ column }) =>
			renderComponent(DataTableNameButton, {
				onclick: column.getToggleSortingHandler()
			})
	},
	{
		accessorKey: 'userId',
		header: 'User Id'
	},
	{
		accessorKey: 'tickerSymbols',
		header: 'Ticker Symbols',
		cell: ({ getValue }) => {
			const tickerSymbols = getValue() as string[] | undefined;
			if (!tickerSymbols) return '';
			return tickerSymbols.join(', ');
		}
	}
];

