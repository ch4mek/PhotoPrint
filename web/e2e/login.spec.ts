import { expect } from '@playwright/test';

import { Routes } from '@/constants/routes';

import { logIn } from './authorization';
import { TIMEOUT, CREDENTIALS } from './config';
import { test } from './fixture';

test.setTimeout(TIMEOUT.XXL);

test('renders logo on sign in page', async ({ page }) => {
	await page.goto(Routes.signIn);
	await expect(page.getByAltText('logo')).toBeVisible({ timeout: TIMEOUT.XXL });
});

test('renders title on sign in page', async ({ page }) => {
	await page.goto(Routes.signIn);
	await expect(page.getByText('Management System')).toBeVisible({
		timeout: TIMEOUT.XXL,
	});
});

test('redirects unauthenticated user from protected route to sign in', async ({
	page,
}) => {
	await page.goto(Routes.booking);
	await expect(page).toHaveURL(Routes.signIn, { timeout: TIMEOUT.XL });
	await expect(page.getByTestId('login-button')).toBeVisible();
});

test('login grants access to protected route', async ({ page }) => {
	const { email, password } = CREDENTIALS.SE;
	await logIn(page, email, password, Routes.booking);
	await expect(page.getByRole('heading', { name: /Bookings/i })).toBeVisible({
		timeout: TIMEOUT.XL,
	});
});

test('returns to originally requested route after login', async ({ page }) => {
	await page.goto(Routes.booking);
	await expect(page).toHaveURL(Routes.signIn, { timeout: TIMEOUT.XL });

	const { email, password } = CREDENTIALS.SE;
	await logIn(page, email, password);
	await expect(page).toHaveURL(Routes.booking, { timeout: TIMEOUT.XL });
});

test('keeps session after reload', async ({ page }) => {
	const { email, password } = CREDENTIALS.SE;
	await logIn(page, email, password, Routes.booking);
	await page.reload();
	await expect(page.getByRole('heading', { name: /Bookings/i })).toBeVisible({
		timeout: TIMEOUT.XL,
	});
});
