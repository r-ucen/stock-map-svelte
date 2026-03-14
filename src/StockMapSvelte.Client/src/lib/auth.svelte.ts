import { goto } from '$app/navigation';
import { resolve } from '$app/paths';
import { apiFetch } from '$lib/apiFetch'

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

	async checkAuth() {
		try {
			const res = await apiFetch('/manage/info');
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
	
	async register(email: string, password: string) {
		const res = await apiFetch('/register?useCookies=true', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({ email, password })
		});
		
		if (res.ok) {
			return { success: true };
		}

		const body = await res.json();
		const errors: string[] = body.errors
			? Object.values<string[]>(body.errors).flat()
			: [body.title ?? 'Registration failed'];
		
		return { success: false, status: res.status, errors };
	}
	
	async confirmEmail(userId: string, code: string) {
		const res = await apiFetch(
			`/confirmEmail?userId=${userId}&code=${code}`
		);
		
		if (res.ok) {
			return { success: true };
		} else {
			const error = 'Email confirmation failed';
			return { success: false, status: res.status, error };
		}
	}

	async login(email: string, password: string) {
		const res = await apiFetch('/login?useCookies=true', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({ email, password })
		});

		if (res.ok) {
			await this.checkAuth();
			const resolvedReturnPath = resolve("/");
			await goto(resolvedReturnPath);
			return { success: true };
		}
		return { success: false, status: res.status };
	}
	
	async logout() {
		await apiFetch('/logout', { method: 'POST' });
		this.#user = null;
		const resolvedLogin = resolve("/login");
		await goto(resolvedLogin);
	}
}

export const auth = new AuthManager();