import type { ColumnDef } from '@tanstack/table-core';
import DataTableActions from './data-table-actions.svelte';
import { renderComponent } from '$lib/components/ui/data-table';
import DataTableNameButton from '$lib/components/Users/data-table-name-button.svelte';
import type { IUser } from '$lib/Abstractions/IUser';

export const columns: ColumnDef<IUser>[] = [
	{
		accessorKey: 'email',
		header: ({ column }) =>
			renderComponent(DataTableNameButton, {
				onclick: column.getToggleSortingHandler()
			})
	},
	{
		accessorKey: 'userName',
		header: 'Username'
	},
	{
		accessorKey: 'roles',
		header: 'Roles',
		cell: ({ getValue }) => {
			const roles = getValue() as string[] | undefined;
			// all authenticated users are customers
			if (!roles || roles.length === 0) return 'Customer';
			return roles.join(', ');
		}
	},
	{
		accessorKey: 'isBanned',
		header: 'Banned',
		cell: ({ getValue }) => {
			const isBanned = getValue() as boolean | undefined;
			return isBanned ? 'Yes' : 'No';
		}
	},
	{
		accessorKey: 'id',
		header: 'Id'
	},
	{
		id: 'actions',
		cell: ({ row }) => {
			return renderComponent(DataTableActions, { id: row.original.id, roles: row.original.roles });
		}
	}
];
