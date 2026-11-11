import { RESPONSE_PHONE_EXIST } from '@/constants/errors';

export const checkIfMessageIsAboutPhoneDuplicate = (message?: string) =>
	message?.includes(RESPONSE_PHONE_EXIST);

export const checkIfMessageIsAboutEmailDuplicate = (message?: string) => {
	const lower = message?.toLowerCase();

	return lower?.includes('email') && lower?.includes('already used');
};

export const checkIfMessageIsAboutRoleDuplicate = (message?: string) =>
	message?.includes('User') && message?.includes('is already exist');
