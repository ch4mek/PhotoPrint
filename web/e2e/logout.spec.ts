import { expect } from '@playwright/test';

import { menuItemsProps } from '@/components/UserMenu/UserMenuDropdown/constants';
import { Routes } from '@/constants/routes';

import { logIn, logOut } from './authorization';
import { TIMEOUT, CREDENTIALS } from './config';
import { test } from './fixture';

test.setTimeout(TIMEOUT.XXL);

test('logout redirects to sign in page', async ({ page }) => {
	const { email, password } = CREDENTIALS.SE;
	await logIn(page, email, password, Routes.booking);
	await logOut(page);
	await expect(page).toHaveURL(Routes.signIn, { timeout: TIMEOUT.XL });
	await expect(page.getByTestId('login-button')).toBeVisible();
});

test('logout clears session storage', async ({ page }) => {
	const { email, password } = CREDENTIALS.SE;
	await logIn(page, email, password, Routes.booking);
	await logOut(page);
	const hasAuth = await page.evaluate(() => !!sessionStorage.getItem('auth'));
	expect(hasAuth).toBeFalsy();
});

test('after logout protected route requires authentication', async ({
	page,
}) => {
	const { email, password } = CREDENTIALS.SE;
	await logIn(page, email, password, Routes.booking);
	await logOut(page);
	await page.goto(Routes.booking);
	await expect(page).toHaveURL(Routes.signIn, { timeout: TIMEOUT.XL });
});

test('logout control is available in user menu', async ({ page }) => {
	const { email, password } = CREDENTIALS.SE;
	await logIn(page, email, password, Routes.booking);
	await page.getByTestId('user-menu').click();
	await expect(
		page
			.getByTestId('user-menu-item-title')
			.filter({ hasText: menuItemsProps.logOut.title })
			.first()
	).toBeVisible();
});
