import { apiFetch } from '$lib/apiFetch';

export const load = async (event) => {
	const res = await apiFetch('/manage/2fa', {
		event,
		method: 'POST',
		headers: {
			'Content-Type': 'application/json'
		},
		body: JSON.stringify({
			enable: null,
			twoFactorCode: null,
			resetSharedKey: false,
			resetRecoveryCodes: false,
			forgetMachine: false
		})
	});

	if (!res.ok) {
		return {
			twoFaInfo: {
				sharedKey: null,
				recoveryCodesLeft: null,
				recoveryCodes: null,
				isTwoFactorEnabled: false,
				isMachineRemembered: false
			}
		};
	}

	return {
		twoFaInfo: await res.json()
	};
};
