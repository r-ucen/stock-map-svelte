import type { ColumnDef } from '@tanstack/table-core';
import type { IPortfolio } from '$lib/Abstractions/IState';
import DataTableActions from './data-table-actions.svelte';
import { renderComponent } from '$lib/components/ui/data-table';
import DataTableNameButton from '$lib/components/Portfolios/data-table-name-button.svelte';

export const columns: ColumnDef<IPortfolio>[] = [
	{
		accessorKey: 'portfolioName',
		header: ({ column }) =>
			renderComponent(DataTableNameButton, {
				onclick: column.getToggleSortingHandler()
			})
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
	},
	{
		id: 'actions',
		cell: ({ row }) => {
			return renderComponent(DataTableActions, { id: row.original.portfolioId });
		}
	}
];
