import { PUBLIC_API_BASE_URL } from '$env/static/public';
import type { RequestEvent } from '@sveltejs/kit';

interface ApiFetchOptions extends RequestInit {
	event?: RequestEvent;
}

export async function apiFetch(endpoint: string, options: ApiFetchOptions = {}) {
	const { event, ...fetchOptions } = options;

	const nativeFetch = event?.fetch ?? fetch;

	const headers = new Headers(fetchOptions.headers);

	if (event) {
		const allCookies = event.request.headers.get('cookie');
		if (allCookies) {
			headers.set('cookie', allCookies);
		}
	}

	const response = await nativeFetch(`${PUBLIC_API_BASE_URL}${endpoint}`, {
		...fetchOptions,
		headers,
		credentials: 'include'
	});

	const contentType = response.headers.get('content-type') || '';
	if (response.status === 429 && contentType.includes('text/html')) {
		throw new Error('VIEW_429');
	}

	return response;
}
