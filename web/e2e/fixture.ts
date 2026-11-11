import { test as base } from '@playwright/test';

import { saveCoverageReport } from './saveCoverageReport';

interface ExtendFixture {
	autoCoverage: void;
}

export const test = base.extend<ExtendFixture>({
	autoCoverage: [
		async ({ page }, use, testInfo) => {
			await use();
			await saveCoverageReport(page, testInfo);
		},
		{ auto: true },
	],
});

export * from '@playwright/test';
