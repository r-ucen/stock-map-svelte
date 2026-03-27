import type { ColumnDef } from '@tanstack/table-core';
import { renderComponent } from '$lib/components/ui/data-table';
import DataTableNameButton from '$lib/components/StockProfiles/data-table-name-button.svelte';
import type { IStockProfile } from '$lib/Abstractions/IStockProfile';

export const columns: ColumnDef<IStockProfile>[] = [
	{
		accessorKey: 'tickerSymbol',
		header: ({ column }) =>
			renderComponent(DataTableNameButton, {
				onclick: column.getToggleSortingHandler()
			})
	},
	{
		accessorKey: 'fullName',
		header: 'Full Name'
	},
    {
		accessorKey: 'sector',
		header: 'Sector'
	},
    {
		accessorKey: 'currency',
		header: 'Currency'
	},
    {
		accessorKey: 'regularMarketChangePercent',
		header: 'Change %'
	},
    {
		accessorKey: 'regularMarketPrice',
		header: 'Price'
	},
    {
        accessorKey: 'volume',
        header: 'Volume'
    }
];

