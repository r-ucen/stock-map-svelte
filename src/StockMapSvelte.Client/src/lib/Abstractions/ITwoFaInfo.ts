export interface ITwoFaInfo {
	sharedKey: string;
	recoveryCodesLeft: number;
	recoveryCodes: string[];
	isTwoFactorEnabled: boolean;
	isMachineRemembered: boolean;
}
