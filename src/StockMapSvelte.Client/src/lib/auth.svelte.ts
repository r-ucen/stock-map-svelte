import { PUBLIC_API_BASE_URL } from '$env/static/public';
import { goto } from '$app/navigation';
import { resolve } from '$app/paths';

export interface User {
	email: string;
	isEmailConfirmed: boolean;
}

class AuthManager {
	#user = $state<User | null>(null);
	#initialized = $state(false);

	get user() {
		return this.#user;
	}

	get isAuthenticated() {
		return !!this.#user;
	}
	get isInitialized() {
		return this.#initialized;
	}

	private async apiFetch(endpoint: string, options: RequestInit = {}) {
		return fetch(`${PUBLIC_API_BASE_URL}${endpoint}`, {
			...options,
			credentials: 'include'
		});
	}

	async checkAuth() {
		try {
			const res = await this.apiFetch('/manage/info');
			if (res.ok) {
				this.#user = await res.json();
			} else {
				this.#user = null;
			}
			// eslint-disable-next-line @typescript-eslint/no-unused-vars
		} catch (e) {
			this.#user = null;
		} finally {
			this.#initialized = true;
		}
	}

	async login(email: string, password: string) {
		const res = await this.apiFetch('/login?useCookies=true', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({ email, password })
		});

		if (res.ok) {
			await this.checkAuth();
			const resolvedAccount = resolve("/account");
			await goto(resolvedAccount);
			return { success: true };
		}
		return { success: false, status: res.status };
	}
	
	async logout() {
		await this.apiFetch('/user/logout', { method: 'POST' });
		this.#user = null;
		const resolvedLogin = resolve("/login");
		await goto(resolvedLogin);
	}
}

export const auth = new AuthManager();