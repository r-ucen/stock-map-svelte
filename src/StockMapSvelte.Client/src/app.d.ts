// See https://svelte.dev/docs/kit/types#app.d.ts
// for information about these interfaces

import type { Login } from '$lib/Abstractions/Login';

declare global {
	namespace App {
		// interface Error {}
		interface Locals {
			user?: {
				email: string;
				isAuthenticated: boolean;
				roles: {
					admin: boolean;
					manager: boolean;
					customer: boolean;
				};
				hasPasswordConfigured: boolean;
				hasExternalLoginConfigured: boolean;
				externalLogins: Array<Login>;
			};
			isBanned: boolean;
		}
		// interface PageData {}
		// interface PageState {}
		// interface Platform {}
	}
}

export {};
