import { JSDOM } from 'jsdom';

import { Roles } from '@/constants/roles';
import {
	getAuthState,
	initialAuthState,
	removeAuthStateFromStorage,
	setAuthState,
} from '@/helpers/localStorage';

const { window } = new JSDOM(`...`, { url: 'http://localhost/' });
global.localStorage = window.localStorage;
global.sessionStorage = window.sessionStorage;

beforeAll(() => {
	const mockGetItem = jest.fn();
	const mockSetItem = jest.fn();
	const mockRemoveItem = jest.fn();
	const mockClear = jest.fn();

	Object.defineProperty(window, 'localStorage', {
		value: {
			getItem: (...args: string[]) => mockGetItem(...args),
			setItem: (...args: string[]) => mockSetItem(...args),
			removeItem: (...args: string[]) => mockRemoveItem(...args),
		},
	});

	Object.defineProperty(window, 'sessionStorage', {
		value: {
			clear: () => mockClear(),
		},
	});
});

describe('Helpers localStorage ', () => {
	test('setAuthState and getAuthState', () => {
		const authState = {
			token: '123',
			role: Roles.SE,
		};
		setAuthState(authState);
		expect(getAuthState()).toEqual(authState);
	});

	test('removeAuthStateFromStorage', () => {
		const authState = {
			token: '123',
			role: Roles.SE,
		};

		setAuthState(authState);

		expect(getAuthState()).toEqual(authState);

		removeAuthStateFromStorage();

		expect(getAuthState()).toEqual(initialAuthState);
	});
});
