/* eslint-disable no-await-in-loop */
import { dataTestIds } from '@mms/mms-ui-library/constants/dataTestIds';
import { APIRequestContext, Locator, Page, expect } from '@playwright/test';

import { initialProjectsValue } from '@/constants/filters';
import { Routes } from '@/constants/routes';
import {
	REPORT_IS_BEING_GENERATED,
	REPORT_IS_GENERATED,
} from '@/constants/staticText';
import dayjs from '@/lib/dayjs';
import {
	REPORTS_LOADER_TEST_ID,
	FILTERS_MESSAGE,
} from '@/pages/Reports/constants';

import { logIn } from './authorization';
import { CREDENTIALS, TIMEOUT } from './config';
import { test } from './fixture';
import { waitForUrlResponse } from './utils';

interface SeedOptions {
	reportMonth?: 'previous' | 'current';
	requireClientName?: boolean;
}

interface LookupUser {
	lastName: string;
	firstName: string;
}

interface UserProjectResponse {
	projectId: number;
	projectName: string;
	clientName: string;
	userId: number;
}

const API_BASE_PREFIX = '/api';
const ENDPOINT_REPORTS = `${API_BASE_PREFIX}/Reports`;
const ENDPOINT_USERS = `${API_BASE_PREFIX}/Users/lookup/filters`;
const CLIENT_PROJECTS_PATH = /\/Clients\/[^/]+\/projects(?=\?|$)/i;
const ENDPOINT_USER_PROJECTS = `${API_BASE_PREFIX}/user-projects`;
const ENDPOINT_TIMESHEET_UPSERT = `${API_BASE_PREFIX}/Timesheet/fact/upsert`;
const ENDPOINT_REPORT_FILE = `${API_BASE_PREFIX}/applicationfile/`;

const AUTOSELECT_TEST_ID = 'autoSelect';
const BARCHART_TEST_ID = 'barchart';
const PIECHART_TEST_ID = 'piechart';
const DROPDOWN_SCROLLBAR_TEST_ID = 'dropdown-scrollbar';
const AUTOSELECT_DROPTAB_TEST_ID = 'autoSelect-dropTab';
const FILTER_BTN_TEST_ID = 'filter-button';

const inputIndexes = {
	client: 0,
	projects: 1,
	users: 2,
	hours: 4,
	period: 6,
} as const;

const PAGE_WIDTH = 1080;
const PAGE_HEIGHT = 1080;

const selectAutocompleteOption = async (
	page: Page,
	input: Locator,
	optionText: string
) => {
	await input.click();
	await page.waitForTimeout(TIMEOUT.S);
	await input.getByRole('textbox').pressSequentially(optionText);
	const dropTabLocator = page.getByTestId(
		`${AUTOSELECT_DROPTAB_TEST_ID}-${optionText}`
	);
	await dropTabLocator.waitFor({ state: 'visible', timeout: TIMEOUT.XL });
	await dropTabLocator.click();
};

const applyFilters = async (page: Page) => {
	await page.getByRole('button', { name: 'Apply' }).click();
};

const requestReportGeneration = async (
	page: Page,
	reportFormat: string = 'pdf'
) => {
	await page.getByTestId(dataTestIds.downloadButton).click();
	await page.getByText(reportFormat).click();
};

const checkNotificationsAndReportAvailability = async (page: Page) => {
	await page.getByTestId('bell-icon').click();

	const notificationsContainer = page.locator(
		'[data-testid="notifications-list"]'
	);
	await expect(notificationsContainer).toBeVisible({ timeout: TIMEOUT.XL });

	await notificationsContainer.locator('[data-testid="loader-icon"]').waitFor({
		state: 'detached',
		timeout: TIMEOUT.XL,
	});

	const generatedInProgressNotification = notificationsContainer
		.getByText(REPORT_IS_BEING_GENERATED, { exact: true })
		.last();

	await generatedInProgressNotification.waitFor({
		state: 'visible',
		timeout: TIMEOUT.L,
	});

	const latestGeneratedNotification = notificationsContainer
		.getByText(REPORT_IS_GENERATED, { exact: true })
		.last();
	await expect(latestGeneratedNotification).toBeVisible({
		timeout: TIMEOUT.XXL,
	});

	const reportDownloadResponsePromise = waitForUrlResponse(page, {
		path: ENDPOINT_REPORT_FILE,
		method: 'GET',
		timeoutMs: TIMEOUT.XL,
	});

	await latestGeneratedNotification.click({ force: true });
	const reportDownloadResponse = await reportDownloadResponsePromise;

	expect(reportDownloadResponse.ok()).toBeTruthy();

	const downloadedFile = await reportDownloadResponse.body();
	expect(downloadedFile.byteLength).toBeGreaterThan(0);
};

const clearNotifications = async (page: Page) => {
	const clearAllButton = page.getByRole('button', { name: 'Clear all' });
	await clearAllButton.click();

	const confirmClearButton = page.getByRole('button', { name: 'Yes' });
	const hasConfirmPopup = await confirmClearButton
		.waitFor({ state: 'visible', timeout: TIMEOUT.S })
		.then(() => true)
		.catch(() => false);

	if (hasConfirmPopup) {
		await confirmClearButton.click();
	}
};

const getAuthToken = async (page: Page) =>
	page.evaluate(() => JSON.parse(sessionStorage.getItem('auth')!).token);

const pickProjectForReports = async (
	token: string,
	apiContext: APIRequestContext,
	requireClientName: boolean
) => {
	const res = await apiContext.get(ENDPOINT_USER_PROJECTS, {
		headers: { Authorization: `Bearer ${token}` },
		params: { Top: 50, Skip: 0 },
	});
	expect(res.ok()).toBeTruthy();

	const { items } = (await res.json()) as {
		items: Array<Partial<UserProjectResponse>>;
	};

	const row = items.find(
		({ projectId, projectName, userId, clientName }) =>
			Boolean(projectId) &&
			Boolean(projectName) &&
			Boolean(userId) &&
			(!requireClientName || Boolean(clientName))
	);

	if (!row?.projectId || !row.projectName || !row.userId) {
		throw new Error(
			`No project for reports (requireClient=${requireClientName}). Check test environment.`
		);
	}

	return {
		projectId: row.projectId,
		projectName: row.projectName,
		clientName: row.clientName ?? '',
		timesheetUserId: row.userId,
	};
};

const getAssignment = async (page: Page, apiContext: APIRequestContext) => {
	const token = await getAuthToken(page);

	return pickProjectForReports(token, apiContext, true);
};

const seedTrackedTime = async (
	page: Page,
	apiContext: APIRequestContext,
	options: SeedOptions = {}
) => {
	const { reportMonth = 'previous', requireClientName = false } = options;
	const token = await getAuthToken(page);
	const pickedProject = await pickProjectForReports(
		token,
		apiContext,
		requireClientName
	);

	const reportForDateAsString = (
		reportMonth === 'current' ? dayjs.utc() : dayjs.utc().subtract(1, 'month')
	)
		.date(15)
		.format('YYYY-MM-DD');

	const upsertResponse = await apiContext.post(ENDPOINT_TIMESHEET_UPSERT, {
		headers: { Authorization: `Bearer ${token}` },
		data: {
			id: 0,
			userId: pickedProject.timesheetUserId,
			projectId: pickedProject.projectId,
			taskName: `e2e-report-seed-${Date.now()}`,
			reportForDate: reportForDateAsString,
			factTrackedInMinutes: 60,
		},
	});
	expect(upsertResponse.ok()).toBeTruthy();

	return pickedProject;
};

const checkDisplayedItems = async (
	page: Page,
	input: Locator,
	endpoint: string | RegExp,
	getText: (item: Record<string, string>) => string,
	options?: {
		triggerResponse?: () => Promise<void>;
	}
) => {
	const responsePromise = waitForUrlResponse(page, {
		path: endpoint,
		timeoutMs: TIMEOUT.XXL,
	});

	if (options?.triggerResponse) {
		await options.triggerResponse();
	} else {
		await input.click();
	}

	const relatedItemsResponse = await responsePromise;
	const relatedItems = (await relatedItemsResponse.json()) as Record<
		string,
		string
	>[];

	if (options?.triggerResponse) {
		await input.click();
	}

	await page.waitForSelector(`[data-testid="${DROPDOWN_SCROLLBAR_TEST_ID}"]`, {
		state: 'visible',
		timeout: TIMEOUT.L,
	});

	const itemsLocator = page
		.getByTestId(DROPDOWN_SCROLLBAR_TEST_ID)
		.locator('li:not([aria-hidden])');
	await expect(itemsLocator.first()).toBeVisible({ timeout: TIMEOUT.L });

	const itemsToDisplay = relatedItems.slice(0, 5);

	await Promise.all(
		itemsToDisplay.map((item) => {
			const expectedText = getText(item);
			const itemLocator = itemsLocator.getByText(expectedText, { exact: true });

			return expect(itemLocator).toBeVisible({ timeout: TIMEOUT.M });
		})
	);
};

test.describe('Reports module', () => {
	let apiContext: APIRequestContext;

	test.beforeAll(async ({ playwright }) => {
		apiContext = await playwright.request.newContext({
			baseURL: process.env.VITE_API_URL,
			extraHTTPHeaders: {
				Accept: 'application/json, text/plain, */*',
			},
		});
	});

	test.afterAll(async () => {
		await apiContext.dispose();
	});

	test.beforeEach(async ({ page }) => {
		const { email, password } = CREDENTIALS.CTO;
		await logIn(page, email, password, Routes.reports);
		await page.setViewportSize({ width: PAGE_WIDTH, height: PAGE_HEIGHT });
		await page.waitForSelector(`[data-id="${REPORTS_LOADER_TEST_ID}"]`, {
			state: 'detached',
		});
		await page.getByTestId(FILTER_BTN_TEST_ID).click();
	});

	test('Generate report in PDF format for the client for the last quarter', async ({
		page,
	}) => {
		test.setTimeout(TIMEOUT.XXL);

		const seeded = await seedTrackedTime(page, apiContext, {
			requireClientName: true,
		});

		const filterAutoselects = page.getByTestId(AUTOSELECT_TEST_ID);
		const clientInput = filterAutoselects.nth(inputIndexes.client);
		const periodInput = filterAutoselects.nth(inputIndexes.period);

		await selectAutocompleteOption(page, clientInput, seeded.clientName);
		await expect(page.getByText(FILTERS_MESSAGE)).toBeVisible();

		await periodInput.click();
		await page.getByTestId(`${AUTOSELECT_DROPTAB_TEST_ID}-Quarter`).click();
		await applyFilters(page);

		await requestReportGeneration(page);
		await checkNotificationsAndReportAvailability(page);
		await clearNotifications(page);
	});

	test('Generate report for a project in PDF format for an employee for the last month', async ({
		page,
	}) => {
		test.setTimeout(TIMEOUT.XXL);

		const seeded = await seedTrackedTime(page, apiContext);

		const filterAutoselects = page.getByTestId(AUTOSELECT_TEST_ID);
		const projectsInput = filterAutoselects.nth(inputIndexes.projects);
		const usersInput = filterAutoselects.nth(inputIndexes.users);

		const usersResponsePromise = waitForUrlResponse(page, {
			path: ENDPOINT_USERS,
			timeoutMs: TIMEOUT.XXL,
		});
		await selectAutocompleteOption(page, projectsInput, seeded.projectName);

		const usersResponse = await usersResponsePromise;
		const users = (await usersResponse.json()) as LookupUser[];
		expect(users.length).toBeGreaterThan(0);
		const user = users[0];

		await usersInput.click();
		await page
			.getByTestId(
				`${AUTOSELECT_DROPTAB_TEST_ID}-${user.lastName} ${user.firstName}`
			)
			.click();
		await applyFilters(page);

		await requestReportGeneration(page);
		await checkNotificationsAndReportAvailability(page);
		await clearNotifications(page);
	});

	test('Resetting filters', async ({ page }) => {
		const assignment = await getAssignment(page, apiContext);

		const filterAutoselects = page.getByTestId(AUTOSELECT_TEST_ID);
		const projectsInput = filterAutoselects.nth(inputIndexes.projects);
		const periodInput = filterAutoselects.nth(inputIndexes.period);

		await selectAutocompleteOption(page, projectsInput, assignment.projectName);
		await expect(page.getByText(FILTERS_MESSAGE)).toBeVisible();

		await periodInput.click();
		await page.getByTestId(`${AUTOSELECT_DROPTAB_TEST_ID}-Quarter`).click();
		await page.getByRole('button', { name: 'Reset' }).click();
		await page.getByTestId(FILTER_BTN_TEST_ID).click();

		await expect(projectsInput.getByRole('textbox')).toHaveValue(
			initialProjectsValue.name
		);
		await expect(periodInput.getByRole('textbox')).toHaveValue('Month');
	});

	test('Find the day on which the largest number of hours worked per month for the project occurred', async ({
		page,
	}) => {
		const seeded = await seedTrackedTime(page, apiContext, {
			reportMonth: 'current',
		});

		const filterAutoselects = page.getByTestId(AUTOSELECT_TEST_ID);
		const hoursInput = filterAutoselects.nth(inputIndexes.hours);
		const projectsInput = filterAutoselects.nth(inputIndexes.projects);

		await hoursInput.click();
		await page.getByTestId(`${AUTOSELECT_DROPTAB_TEST_ID}-Fact`).click();

		await selectAutocompleteOption(page, projectsInput, seeded.projectName);

		const reportsResponsePromise = waitForUrlResponse(page, {
			path: ENDPOINT_REPORTS,
		});
		await applyFilters(page);
		const reportsResponse = await reportsResponsePromise;
		expect((await reportsResponse.json()).totalTimeInMinutes).toBeGreaterThan(
			0
		);

		await expect(page.getByTestId(BARCHART_TEST_ID)).toBeVisible({
			timeout: TIMEOUT.XL,
		});
	});

	test('Display piecharts', async ({ page }) => {
		const filterAutoselects = page.getByTestId(AUTOSELECT_TEST_ID);
		const periodInput = filterAutoselects.nth(inputIndexes.period);

		await periodInput.click();
		await page.getByTestId(`${AUTOSELECT_DROPTAB_TEST_ID}-Year`).click();

		await expect(page.getByText(FILTERS_MESSAGE)).toBeVisible();
		await applyFilters(page);

		await page.waitForLoadState('networkidle');

		const piecharts = page.getByTestId(PIECHART_TEST_ID);

		await expect(piecharts).toHaveCount(3, { timeout: TIMEOUT.XL });

		const charts = await piecharts.all();
		await Promise.all(
			charts.map((chart) => expect(chart).toBeVisible({ timeout: TIMEOUT.L }))
		);
	});

	test('Dependence of the Project filter on the Client filter', async ({
		page,
	}) => {
		const assignment = await getAssignment(page, apiContext);
		const filterAutoselects = page.getByTestId(AUTOSELECT_TEST_ID);
		const clientInput = filterAutoselects.nth(inputIndexes.client);
		const projectsInput = filterAutoselects.nth(inputIndexes.projects);

		await checkDisplayedItems(
			page,
			projectsInput,
			CLIENT_PROJECTS_PATH,
			(item) => item.name,
			{
				triggerResponse: () =>
					selectAutocompleteOption(page, clientInput, assignment.clientName),
			}
		);
	});

	test('Dependence of the Users filter on the Client filter where only Private project relates to Client (for filter)', async ({
		page,
	}) => {
		const assignment = await getAssignment(page, apiContext);
		const filterAutoselects = page.getByTestId(AUTOSELECT_TEST_ID);
		const clientInput = filterAutoselects.nth(inputIndexes.client);
		const usersInput = filterAutoselects.nth(inputIndexes.users);

		await checkDisplayedItems(
			page,
			usersInput,
			ENDPOINT_USERS,
			({ lastName, firstName }) => `${lastName} ${firstName}`,
			{
				triggerResponse: () =>
					selectAutocompleteOption(page, clientInput, assignment.clientName),
			}
		);
	});

	test('Dependence of the Users filter on the Project filter with Private project (for filter)', async ({
		page,
	}) => {
		const assignment = await getAssignment(page, apiContext);
		const filterAutoselects = page.getByTestId(AUTOSELECT_TEST_ID);
		const projectsInput = filterAutoselects.nth(inputIndexes.projects);
		const usersInput = filterAutoselects.nth(inputIndexes.users);

		await checkDisplayedItems(
			page,
			usersInput,
			ENDPOINT_USERS,
			({ lastName, firstName }) => `${lastName} ${firstName}`,
			{
				triggerResponse: () =>
					selectAutocompleteOption(page, projectsInput, assignment.projectName),
			}
		);
	});

	test('Dependence of the Users filter on the Project filter with Public project (for filter)', async ({
		page,
	}) => {
		const token = await getAuthToken(page);
		const res = await apiContext.get(`${API_BASE_PREFIX}/Projects/lookup`, {
			headers: { Authorization: `Bearer ${token}` },
			params: { isPublic: true, isActive: true },
		});
		expect(res.ok()).toBeTruthy();
		const publicProjects = (await res.json()) as Array<{ name: string }>;

		if (!publicProjects.length) {
			throw new Error('No public projects in test environment. Seed required.');
		}

		const publicProjectName = publicProjects[0].name;

		const filterAutoselects = page.getByTestId(AUTOSELECT_TEST_ID);
		const usersInput = filterAutoselects.nth(inputIndexes.users);
		const projectsInput = filterAutoselects.nth(inputIndexes.projects);

		const usersResponsePromise = waitForUrlResponse(page, {
			path: ENDPOINT_USERS,
			timeoutMs: TIMEOUT.XXL,
		});

		await selectAutocompleteOption(page, projectsInput, publicProjectName);

		const usersResponse = await usersResponsePromise;
		const users = (await usersResponse.json()) as LookupUser[];
		expect(users.length).toBeGreaterThan(0);

		await usersInput.click();

		await page.waitForSelector(
			`[data-testid="${DROPDOWN_SCROLLBAR_TEST_ID}"]`,
			{
				state: 'visible',
				timeout: TIMEOUT.L,
			}
		);

		const itemsLocator = page
			.getByTestId(DROPDOWN_SCROLLBAR_TEST_ID)
			.locator('li:not([aria-hidden])');
		await expect(itemsLocator.first()).toBeVisible({ timeout: TIMEOUT.L });

		const apiUserNames = new Set(
			users.map((u) => `${u.lastName} ${u.firstName}`)
		);
		const visibleTexts = await itemsLocator.allTextContents();
		const matchCount = visibleTexts.filter((text) =>
			apiUserNames.has(text.trim())
		).length;
		expect(matchCount).toBeGreaterThan(0);
	});
});
