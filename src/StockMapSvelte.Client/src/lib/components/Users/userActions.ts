import { apiFetch } from '$lib/apiFetch';
import type { IAdminState } from '$lib/Abstractions/IAdminState';
import type { IUser } from '$lib/Abstractions/IUser';

export async function updateUserRoles(state: IAdminState, id: string, newRoles: string[]) {
	const res = await apiFetch(`/users/roles`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json',
			Accept: 'application/json'
		},
		body: JSON.stringify({
			newRoles: newRoles,
			userId: id
		})
	});

	if (res.ok) {
		return { success: true };
	} else {
		const errorData = await res.json().catch(() => ({}));
		const error = errorData.message || 'Failed to update roles';
		return { success: false, status: res.status, error };
	}
}

export async function deleteUser(state: IAdminState, id: string) {
	const res = await apiFetch(`/users/${id}`, {
		method: 'DELETE'
	});

	if (res.ok) {
		state.refreshUsers++;
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

export function getUserById(users: IUser[], id: string): IUser | undefined {
	return users.find((p) => p.id === id);
}

export async function banUser(state: IAdminState, id: string) {
	const res = await apiFetch(`/users/ban/${id}`, {
		method: 'POST'
	});

	if (res.ok) {
		state.refreshUsers++;
		return { success: true };
	} else {
		const errorData = await res.json().catch(() => ({}));
		const error = errorData.message || 'Failed to ban user';
		return { success: false, status: res.status, error };
	}
}

export async function unbanUser(state: IAdminState, id: string) {
	const res = await apiFetch(`/users/unban/${id}`, {
		method: 'POST'
	});

	if (res.ok) {
		state.refreshUsers++;
		return { success: true };
	} else {
		const errorData = await res.json().catch(() => ({}));
		const error = errorData.message || 'Failed to unban user';
		return { success: false, status: res.status, error };
	}
}
