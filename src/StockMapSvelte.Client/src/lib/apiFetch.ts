import { PUBLIC_API_BASE_URL } from '$env/static/public';

export async function apiFetch(endpoint: string, options: RequestInit = {}) {
	return fetch(`${PUBLIC_API_BASE_URL}${endpoint}`, {
		...options,
		credentials: 'include'
	});
}