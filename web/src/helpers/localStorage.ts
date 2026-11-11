import { SESSION_EXPIRES } from '@/constants/authConstants';
import { Roles } from '@/constants/roles';
import { AUTH_STORAGE_KEY } from '@/constants/storageKeys';
import type { InitialAuthState } from '@/context/types';

export const initialAuthState: InitialAuthState = {
	token: null,
	role: Roles.SE,
};

export const removeAuthStateFromStorage = () => {
	sessionStorage.removeItem(AUTH_STORAGE_KEY);
	sessionStorage.removeItem(SESSION_EXPIRES);
};

export const getAuthState = (): InitialAuthState => {
	let state = initialAuthState;
	try {
		const storedAuthState = sessionStorage.getItem(AUTH_STORAGE_KEY);

		if (storedAuthState) {
			state = JSON.parse(storedAuthState);
		}

		return state;
	} catch (error) {
		return state;
	}
};

export const setAuthState = (authState: Partial<InitialAuthState>): void => {
	const state = { ...getAuthState(), ...authState };
	const serializedState = JSON.stringify(state);

	sessionStorage.setItem(AUTH_STORAGE_KEY, serializedState);
};
