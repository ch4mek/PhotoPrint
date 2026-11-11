import { Page } from '@playwright/test';

import { menuItemsProps } from '@/components/UserMenu/UserMenuDropdown/constants';
import { Routes } from '@/constants/routes';

import { AUTH_HOST, TIMEOUT } from './config';

interface PopupCloseOptions {
	readonly maxAttempts?: number;
	readonly appearanceTimeout?: number;
	readonly expectedStateAfterClick?: 'hidden' | 'detached';
}

const CLOSE_POPUP_MAX_ATTEMPTS = 3;

const closePopupIfVisible = async (
	page: Page,
	options: PopupCloseOptions = {}
): Promise<void> => {
	const {
		maxAttempts = CLOSE_POPUP_MAX_ATTEMPTS,
		appearanceTimeout = TIMEOUT.L,
		expectedStateAfterClick = 'hidden',
	} = options;

	await page.waitForLoadState('domcontentloaded');

	const closeButton = page.getByTestId('popUp-cross-button');

	const attemptClose = async (attemptCount: number): Promise<void> => {
		if (attemptCount >= maxAttempts) {
			return;
		}

		const isPopupVisible = await closeButton
			.waitFor({ state: 'visible', timeout: appearanceTimeout })
			.then(() => true)
			.catch(() => false);

		if (!isPopupVisible) {
			return;
		}

		const wasClickSuccessful = await closeButton
			.click({ timeout: appearanceTimeout })
			.then(() => true)
			.catch(() => false);

		if (!wasClickSuccessful) {
			return attemptClose(attemptCount + 1);
		}

		const isPopupClosed = await closeButton
			.waitFor({ state: expectedStateAfterClick, timeout: appearanceTimeout })
			.then(() => true)
			.catch(() => false);

		if (!isPopupClosed) {
			return attemptClose(attemptCount + 1);
		}
	};

	await attemptClose(0);
};

export async function logIn(
	page: Page,
	loginText: string,
	password: string,
	redirectTo: string = Routes.booking
) {
	await page.goto(Routes.signIn, {
		waitUntil: 'domcontentloaded',
	});
	const loginButton = page.getByTestId('login-button');
	await loginButton.waitFor({
		state: 'visible',
		timeout: TIMEOUT.XXL,
	});
	await loginButton.click({ timeout: TIMEOUT.XL });

	await page
		.waitForURL((url) => url.hostname.includes(AUTH_HOST), {
			timeout: TIMEOUT.XXL,
		})
		.catch(() => {});

	const loginInput = page.getByPlaceholder('Email, phone, or Skype');

	await loginInput.waitFor({ state: 'visible', timeout: TIMEOUT.XL });
	await loginInput.click();
	await loginInput.fill(loginText);
	await page.getByRole('button', { name: /Next/i }).click();

	const passwordInput = page.getByLabel(/Password/i);
	await passwordInput.waitFor({ state: 'visible', timeout: TIMEOUT.XL });
	await passwordInput.fill(password);
	await page.getByRole('button', { name: /Sign in/i }).click();

	await page.getByRole('button', { name: /No/i }).first().click();

	await page.waitForFunction(() => sessionStorage.getItem('auth') !== null, {
		timeout: TIMEOUT.XL,
	});

	if (redirectTo) {
		await page
			.waitForLoadState('networkidle', { timeout: TIMEOUT.M })
			.catch(() => {});

		await page.goto(redirectTo, {
			waitUntil: 'domcontentloaded',
			timeout: TIMEOUT.XXL,
		});

		if (redirectTo === Routes.booking) {
			await closePopupIfVisible(page);
		}
	}
}

export async function logOut(page: Page) {
	await page.getByTestId('user-menu').click();

	await page
		.getByTestId('user-menu-item-title')
		.filter({ hasText: menuItemsProps.logOut.title })
		.click();
}
