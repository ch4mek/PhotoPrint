import { expect, Page, Locator } from '@playwright/test';

import { DATE_SEPARATOR } from '@/constants/date';
import { BOOKING_LIST_LOADER_TEST_ID } from '@/pages/Booking/constants';

import { logIn } from './authorization';
import { CREDENTIALS, TIMEOUT } from './config';
import { test } from './fixture';
import {
	ALREADY_HAVE_BOOKING_WARNING_TEXT,
	BOOK_DESK_BUTTON_TEXT,
	BLOCK_DESK_BUTTON_TEXT,
	BOOKINGS_TAB_TEXT,
	DESK_BOOKED_TOAST_TEXT,
	DETAILS_TAB_TEXT,
	GUEST_ALREADY_BOOKED_WARNING_TEXT,
	GUESTS_MODE_TEXT,
	TO_DATE_PLACEHOLDER,
	UNBLOCK_DESK_CONFIRM_TEXT,
	YES_BUTTON_TEXT,
} from './translations';

const FIND_AVAILABLE_DESK_MAX_ATTEMPTS = 10;
const SLASH_REGEX = /\//g;

test.setTimeout(TIMEOUT.XXL);

const getBookButton = (page: Page, text: string = BOOK_DESK_BUTTON_TEXT) =>
	page.getByTestId('base-button').filter({ hasText: text }).first();

const getAvailableDeskTypography = (page: Page) =>
	page
		.getByTestId('desk')
		.getByTestId('typography')
		.filter({ hasNot: page.getByTestId('photo') });

const getRepeatSwitch = (page: Page) =>
	page.locator('[for="repeat-switch"]').first();

const setTempId = async (target: Locator, tempId: string) =>
	target.evaluate((el, id) => el.setAttribute('data-temp-id', id), tempId);

const clearTempId = async (page: Page, tempId: string) =>
	page
		.locator(`[data-temp-id="${tempId}"]`)
		.evaluate((el) => el.removeAttribute('data-temp-id'));

const formatDateEnGB = (date: Date) =>
	date
		.toLocaleDateString('en-GB', {
			day: '2-digit',
			month: '2-digit',
			year: 'numeric',
		})
		.replace(SLASH_REGEX, DATE_SEPARATOR);

const expectAlreadyHasBookingPopup = async (page: Page) => {
	const popup = page.getByTestId('popUp-content');
	await expect(popup).toBeVisible({ timeout: TIMEOUT.XL });
	await expect(popup).toHaveText(ALREADY_HAVE_BOOKING_WARNING_TEXT);
	await page.getByTestId('popUp-cross-button').first().click();
};

async function findAvailableDesk(
	page: Page,
	attempt = 0,
	maxAttempts = FIND_AVAILABLE_DESK_MAX_ATTEMPTS
): Promise<Locator> {
	if (page.isClosed()) {
		return page.getByTestId('desk').first();
	}

	if (attempt >= maxAttempts) {
		return page.getByTestId('desk').first();
	}

	const deskTypography = getAvailableDeskTypography(page).nth(attempt);

	if ((await deskTypography.count()) === 0) {
		return findAvailableDesk(page, attempt + 1, maxAttempts);
	}

	await deskTypography.click();

	if ((await deskTypography.getByTestId('photo').count()) === 0) {
		return deskTypography;
	}

	return findAvailableDesk(page, attempt + 1, maxAttempts);
}

const createSingleBooking = async (
	page: Page,
	isCheckingBooking: boolean = true,
	isNeedConfirm: boolean = false,
	buttonText: string = BOOK_DESK_BUTTON_TEXT
) => {
	const targetDesk = await findAvailableDesk(page);
	const tempId = 'desk-current-test';

	await setTempId(targetDesk, tempId);

	const bookButton = getBookButton(page, buttonText);
	await expect(bookButton).toBeVisible();
	await expect(bookButton).toBeEnabled();
	await bookButton.click();

	if (isNeedConfirm) {
		const popup = page.getByTestId('popUp');
		await popup.waitFor({ state: 'visible' });
		const yesButton = popup.getByRole('button', { name: YES_BUTTON_TEXT });
		await expect(yesButton).toBeVisible();
		await yesButton.click();
	}

	if (isCheckingBooking) {
		const bookedDesk = page.locator(`[data-temp-id="${tempId}"]`);
		await expect(bookedDesk.getByTestId('photo')).toBeVisible({
			timeout: TIMEOUT.XL,
		});

		await clearTempId(page, tempId);

		const successMessage = page.getByText(DESK_BOOKED_TOAST_TEXT, {
			exact: true,
		});
		await expect(successMessage).toBeVisible({ timeout: TIMEOUT.XL });
	} else {
		await clearTempId(page, tempId);
	}
};

const createMultipleBooking = async (
	page: Page,
	isCheckingBooking: boolean = true
) => {
	const tomorrow = new Date();
	tomorrow.setDate(tomorrow.getDate() + 1);
	const formattedDate = formatDateEnGB(tomorrow);

	const toInput = page.getByPlaceholder(TO_DATE_PLACEHOLDER);
	await toInput.fill(formattedDate);

	const target = getAvailableDeskTypography(page).first();

	const tempId = 'desk-current-test';
	await setTempId(target, tempId);

	await target.click();
	await page.waitForTimeout(TIMEOUT.M);

	const bookButton = getBookButton(page);
	await expect(bookButton).toBeVisible();
	await expect(bookButton).toBeEnabled();
	await page.waitForTimeout(TIMEOUT.M);
	await bookButton.click();

	if (isCheckingBooking) {
		const bookedDesk = page.locator(`[data-temp-id="${tempId}"]`);
		await expect(bookedDesk.getByTestId('photo')).toBeVisible({
			timeout: TIMEOUT.XL,
		});

		await clearTempId(page, tempId);

		const successMessage = page.getByText(DESK_BOOKED_TOAST_TEXT, {
			exact: true,
		});
		await expect(successMessage).toBeVisible({ timeout: TIMEOUT.XL });
	}
};

const createRepetitiveBooking = async (
	page: Page,
	isCheckingBooking: boolean = true
) => {
	const target = getAvailableDeskTypography(page).first();

	const tempId = 'desk-current-test';
	await setTempId(target, tempId);

	await target.click();
	await getRepeatSwitch(page).click();

	await page.getByTestId('counter-increment-button').first().click();

	const bookButton = getBookButton(page);
	await expect(bookButton).toBeVisible();
	await expect(bookButton).toBeEnabled();
	await page.waitForTimeout(TIMEOUT.M);
	await bookButton.click();

	if (isCheckingBooking) {
		const bookedDesk = page.locator(`[data-temp-id="${tempId}"]`);
		await expect(bookedDesk.getByTestId('photo')).toBeVisible({
			timeout: TIMEOUT.XL,
		});

		await clearTempId(page, tempId);

		const successMessage = page.getByText(DESK_BOOKED_TOAST_TEXT, {
			exact: true,
		});
		await expect(successMessage).toBeVisible({ timeout: TIMEOUT.XL });
	}
};

const removeAllBookings = async (page: Page) => {
	const crossIcons = page.getByTestId('booking-list').getByTestId('cross-icon');

	if ((await crossIcons.count()) === 0) {
		return;
	}

	const firstCrossIcon = crossIcons.first();

	if (await firstCrossIcon.isVisible()) {
		await firstCrossIcon.click();
		await page.getByRole('button', { name: YES_BUTTON_TEXT }).click();

		await page.waitForTimeout(TIMEOUT.M);

		await removeAllBookings(page);
	}
};

const prepareBookingDesks = async (page: Page) => {
	await page.getByRole('button', { name: BOOKINGS_TAB_TEXT }).click();

	await page.waitForSelector(`[data-id="${BOOKING_LIST_LOADER_TEST_ID}"]`, {
		state: 'detached',
	});
	await page.waitForTimeout(TIMEOUT.M);

	await removeAllBookings(page);

	await page.getByRole('button', { name: DETAILS_TAB_TEXT }).click();
};

test.describe('Emploee bookings', () => {
	test.beforeEach(async ({ page }) => {
		const { email, password } = CREDENTIALS.SE;
		await logIn(page, email, password);
		await prepareBookingDesks(page);
	});

	test('single booking creating', async ({ page }) => {
		await createSingleBooking(page);

		const successMessage = page.getByText(DESK_BOOKED_TOAST_TEXT, {
			exact: true,
		});
		await expect(successMessage).toBeVisible({ timeout: TIMEOUT.L });

		const updatedElement = page
			.getByTestId('desk')
			.getByTestId('typography')
			.filter({ has: page.getByTestId('photo') })
			.first();
		await expect(updatedElement).toBeVisible();
	});

	test('multiple  booking creating', async ({ page }) => {
		await createMultipleBooking(page);

		const successMessage = page.getByText(DESK_BOOKED_TOAST_TEXT, {
			exact: true,
		});
		await expect(successMessage).toBeVisible();
	});

	test('repetitive  booking creating', async ({ page }) => {
		await createRepetitiveBooking(page);

		const successMessage = page.getByText(DESK_BOOKED_TOAST_TEXT, {
			exact: true,
		});
		await expect(successMessage).toBeVisible({ timeout: TIMEOUT.XL });
	});

	test('unavailability of single booking creating  for period where single booking of other desk for the same User exists', async ({
		page,
	}) => {
		await createSingleBooking(page);
		await createSingleBooking(page, false);
		await expectAlreadyHasBookingPopup(page);
	});

	test('unavailability of single booking creating  for period where multiple booking of other desk for the same User exists', async ({
		page,
	}) => {
		await createMultipleBooking(page);
		await createSingleBooking(page, false);
		await expectAlreadyHasBookingPopup(page);
	});

	test('unavailability of single booking creating  for period where repetitive booking of other desk for the same User exists', async ({
		page,
	}) => {
		await createRepetitiveBooking(page);
		await createSingleBooking(page, false);
		await expectAlreadyHasBookingPopup(page);
	});

	test('unavailability of multiple  booking creating  for period where sinlgle booking of other desk for the same User exists', async ({
		page,
	}) => {
		await createSingleBooking(page);
		await createMultipleBooking(page, false);
		await expectAlreadyHasBookingPopup(page);
	});

	test('unavailability of multiple  booking creating  for period where multiple booking of other desk for the same User exists', async ({
		page,
	}) => {
		await createMultipleBooking(page);
		await createMultipleBooking(page, false);
		await expectAlreadyHasBookingPopup(page);
	});

	test('unavailability of multiple  booking creating  for period where reprtitive booking of other desk for the same User exists', async ({
		page,
	}) => {
		await createRepetitiveBooking(page);
		await createMultipleBooking(page, false);
		await expectAlreadyHasBookingPopup(page);
	});

	test('unavailability of repetitive  booking creating  for period where sinlgle booking of other desk for the same User exists', async ({
		page,
	}) => {
		await createSingleBooking(page);
		await createRepetitiveBooking(page, false);
		await expectAlreadyHasBookingPopup(page);
	});

	test('unavailability of repetitive  booking creating  for period where multiple booking of other desk for the same User exists', async ({
		page,
	}) => {
		await createMultipleBooking(page);
		await page.reload();
		await createRepetitiveBooking(page, false);
		await expectAlreadyHasBookingPopup(page);
	});

	test('unavailability of repetitive  booking creating  for period where repetitive booking of other desk for the same User exists', async ({
		page,
	}) => {
		await createRepetitiveBooking(page);
		await createRepetitiveBooking(page, false);
		await expectAlreadyHasBookingPopup(page);
	});
});

test.describe('HoD bookings', () => {
	test.beforeEach(async ({ page }) => {
		const { email, password } = CREDENTIALS.HD;
		await logIn(page, email, password);
		await prepareBookingDesks(page);
	});

	test('to book a desk to another person', async ({ page }) => {
		await page.getByTestId('autoSelect').click();

		await page.getByTestId('autoSelect-popover').waitFor({ state: 'visible' });
		const dropdownScrollbar = page.locator('#dropdown-scrollbar');

		const availableItems = dropdownScrollbar.locator(
			'li:not(#selected-autoselect-item)'
		);
		await availableItems.first().click();

		await createSingleBooking(page);
	});

	test('booking for guest', async ({ page }) => {
		const guestsLabel = page
			.getByTestId('typography')
			.filter({ hasText: GUESTS_MODE_TEXT });
		await guestsLabel.click();

		const guestInput = page.locator('#guestName');
		await guestInput.waitFor({ state: 'visible' });
		await guestInput.fill('IntTest User');

		await createSingleBooking(page);
	});

	test('booking for guest with the same name', async ({ page }) => {
		const guestsLabel = page
			.getByTestId('typography')
			.filter({ hasText: GUESTS_MODE_TEXT });
		await guestsLabel.click();

		const guestInput = page.locator('#guestName');
		await guestInput.waitFor({ state: 'visible' });
		await guestInput.fill('IntTest User');

		await createSingleBooking(page);

		await guestInput.fill('IntTest User');
		await page.waitForTimeout(TIMEOUT.M);

		await createSingleBooking(page, false);

		const popup = page.getByTestId('popUp-content');
		await expect(popup).toBeVisible({ timeout: TIMEOUT.L });
		await expect(popup).toHaveText(GUEST_ALREADY_BOOKED_WARNING_TEXT);
		await page.getByTestId('popUp-cross-button').first().click();
	});
});

test.describe('CEO bookings', () => {
	test.beforeEach(async ({ page }) => {
		const { email, password } = CREDENTIALS.CEO;
		await logIn(page, email, password);
		await prepareBookingDesks(page);
	});

	test('to block a work place', async ({ page }) => {
		await createSingleBooking(page, false, true, BLOCK_DESK_BUTTON_TEXT);
	});

	test('to block the existing booking', async ({ page }) => {
		const target = getAvailableDeskTypography(page).first();

		const tempId = 'desk-current-test';
		await setTempId(target, tempId);

		await target.click();

		const bookButton = getBookButton(page);
		await expect(bookButton).toBeVisible();
		await expect(bookButton).toBeEnabled();
		await bookButton.click();

		const bookedDesk = page.locator(`[data-temp-id="${tempId}"]`);

		await expect(bookedDesk.getByTestId('photo')).toBeVisible({
			timeout: TIMEOUT.XL,
		});

		const successMessage = page.getByText(DESK_BOOKED_TOAST_TEXT, {
			exact: true,
		});
		await expect(successMessage).toBeVisible({ timeout: TIMEOUT.XL });

		await bookedDesk.click();

		await expect(
			page
				.getByTestId('base-button')
				.filter({ hasText: BLOCK_DESK_BUTTON_TEXT })
				.first()
		).toBeDisabled();

		await expect(bookedDesk.getByTestId('photo')).toBeVisible({
			timeout: TIMEOUT.XL,
		});

		await clearTempId(page, tempId);
	});

	test('to delete the existing booking', async ({ page }) => {
		await createSingleBooking(page);

		await page.getByRole('button', { name: BOOKINGS_TAB_TEXT }).click();

		const crossIcon = page
			.getByTestId('booking-list')
			.getByTestId('cross-icon')
			.first();
		await crossIcon.click();
		await page.getByRole('button', { name: YES_BUTTON_TEXT }).click();
	});

	test('to unblock a desk', async ({ page }) => {
		await createSingleBooking(page, false, true, BLOCK_DESK_BUTTON_TEXT);

		await page.getByRole('button', { name: BOOKINGS_TAB_TEXT }).click();

		const crossIcon = page
			.getByTestId('booking-list')
			.getByTestId('cross-icon')
			.first();
		await crossIcon.click();

		await expect(page.getByTestId('popUp-content')).toHaveText(
			UNBLOCK_DESK_CONFIRM_TEXT
		);
		await page.getByRole('button', { name: YES_BUTTON_TEXT }).click();
	});

	test('to multiple book a desk with single booking', async ({ page }) => {
		const target = getAvailableDeskTypography(page).first();

		const tempId = 'desk-current-test';
		await setTempId(target, tempId);

		await target.click();

		const tomorrow = new Date();
		tomorrow.setDate(tomorrow.getDate() + 1);
		const formattedDate = formatDateEnGB(tomorrow);

		const toInput = page.getByPlaceholder(TO_DATE_PLACEHOLDER);
		await toInput.fill(formattedDate);

		const bookButton = getBookButton(page);
		await expect(bookButton).toBeVisible();
		await expect(bookButton).toBeEnabled();

		await bookButton.click();

		const bookedDesk = page.locator(`[data-temp-id="${tempId}"]`);
		await expect(bookedDesk.getByTestId('photo')).toBeVisible({
			timeout: TIMEOUT.XL,
		});

		await clearTempId(page, tempId);
	});

	test('user tries to make a repetitive booking', async ({ page }) => {
		const target = getAvailableDeskTypography(page).first();

		const tempId = 'desk-current-test';
		await setTempId(target, tempId);

		await target.click();

		await getRepeatSwitch(page).click();

		const bookButton = getBookButton(page);
		await expect(bookButton).toBeVisible();
		await bookButton.click();

		const bookedDesk = page.locator(`[data-temp-id="${tempId}"]`);

		await expect(bookedDesk.getByTestId('photo')).toBeVisible({
			timeout: TIMEOUT.XL,
		});

		await getRepeatSwitch(page).click();
		await page.getByTestId('counter-increment-button').first().click();

		await bookButton.scrollIntoViewIfNeeded();
		await expect(bookButton).toBeDisabled();

		await clearTempId(page, tempId);
	});
});
