import { apiFetch } from '$lib/apiFetch';
import type { IAdminState } from '$lib/Abstractions/IAdminState';
import type { IUser } from '$lib/Abstractions/IUser';

export async function deleteUser(state: IAdminState, id: string) {
	const res = await apiFetch(`/users/${id}`, {
		method: 'DELETE'
	});

	if (res.ok) {
		deleteUserInState(state, id);
		return { success: true };
	} else {
		const errorData = await res.json().catch(() => ({}));
		const error = errorData.message || 'Failed to delete user';
		return { success: false, status: res.status, error };
	}
}

function deleteUserInState(state: IAdminState, id: string) {
	state.users = state.users.filter((s) => s.id !== id);
}

export function getUserById(users: IUser[], id: string) : IUser | undefined {
	return users.find((p) => p.id === id);
}
