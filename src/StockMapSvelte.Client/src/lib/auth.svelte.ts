import { goto } from '$app/navigation';
import { resolve } from '$app/paths';
import { apiFetch } from '$lib/apiFetch';

class AuthManager {
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
	
	async changePassword(oldPassword: string, newPassword: string) {
		const res = await apiFetch('/manage/info', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify({
				newEmail: null,
				oldPassword,
				newPassword
			})
		});
		
		if (res.ok) {
			return { success: true };
		} else {
			const body = await res.json();
			const errors: string[] = body.errors
				? Object.values<string[]>(body.errors).flat()
				: [body.title ?? body.detail ?? 'Password change failed'];
			return { success: false, status: res.status, errors };
		}
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
	
	async forgotPassword(email: string) {
		const res = await apiFetch('/forgotPassword', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({ email })
		});
		
		if (res.ok) {
			return { success: true };
		} else {
			const error = 'Password reset request failed';
			return { success: false, status: res.status, error };
		}
	}
	
	async resendEmailConfirmation(email: string) {
		const res = await apiFetch('/resendConfirmationEmail', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({ email })
		});
		
		if (res.ok) {
			return { success: true };
		} else {
			const error = 'Resend email confirmation failed';
			return { success: false, status: res.status, error };
		}
	}

	async resetPassword(email: string, code: string, newPassword: string) {
		const res = await apiFetch('/resetPassword', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({ email, resetCode: code, newPassword })
		});
		
		if (res.ok) {
			return { success: true };
		} else {
			const error = 'Password reset failed';
			return { success: false, status: res.status, error };
		}
	};
	
	async login(email: string, password: string) {
		const res = await apiFetch('/login?useCookies=true', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({ email, password })
		});

		if (res.ok) {
			const resolvedReturnPath = resolve("/map");
			await goto(resolvedReturnPath);
			return { success: true };
		}

		let msg: string | any = 'Invalid credentials';
		if (res.status === 429) {
			msg = 'Too many login attempts. Please try again later.';
		} else if (res.status === 401) {
			try {
				const body = await res.json();
				if (body.detail === 'RequiresTwoFactor') {
					msg = 'RequiresTwoFactor';
					return { success: false, status: res.status, error: msg, requiresTwoFactor: true };
				} else if (body.detail === 'LockedOut') {
					msg = 'Too many login attempts. Please try again later.';
				}
				// eslint-disable-next-line @typescript-eslint/no-unused-vars
			} catch (err) {
				msg = 'Invalid credentials';
			}
		}
		return { success: false, status: res.status, error: msg };
	}
	
	async logout() {
		await apiFetch('/logout', { method: 'POST' });
		const resolvedLogin = resolve("/");
		await goto(resolvedLogin);
	}
}

export const auth = new AuthManager();