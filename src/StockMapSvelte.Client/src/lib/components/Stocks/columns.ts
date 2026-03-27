import type { ColumnDef } from '@tanstack/table-core';
import DataTableActions from './data-table-actions.svelte';
import { renderComponent } from '$lib/components/ui/data-table';
import DataTableNameButton from '$lib/components/Stocks/data-table-name-button.svelte';
import type { IStock } from '$lib/Abstractions/IStock';

export const columns: ColumnDef<IStock>[] = [
	{
		accessorKey: 'tickerSymbol',
		header: ({ column }) =>
			renderComponent(DataTableNameButton, {
				onclick: column.getToggleSortingHandler()
			})
	},
	{
		accessorKey: 'id',
		header: 'Id'
	},
	{
		id: 'actions',
		cell: ({ row }) => {
			return renderComponent(DataTableActions, { id: row.original.id });
		}
	}
];
