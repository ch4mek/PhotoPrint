import type { Page, Response } from '@playwright/test';

type HttpMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';

interface ApiWaitOptions {
	readonly path: string | RegExp;
	readonly method?: HttpMethod;
	readonly expectedStatuses?: number | number[];
	readonly timeoutMs?: number;
}

const isMatchingUrl = (url: string, pattern: string | RegExp): boolean =>
	typeof pattern === 'string' ? url.includes(pattern) : pattern.test(url);

const isMatchingStatus = (
	actualStatus: number,
	expectedStatuses?: number | number[]
): boolean => {
	if (!expectedStatuses) {
		return actualStatus >= 200 && actualStatus < 300;
	}

	const statusesArray = Array.isArray(expectedStatuses)
		? expectedStatuses
		: [expectedStatuses];

	return statusesArray.includes(actualStatus);
};

export const waitForUrlResponse = async (
	page: Page,
	options: ApiWaitOptions
): Promise<Response> => {
	const { path, method, expectedStatuses, timeoutMs } = options;

	return page.waitForResponse(
		(response) => {
			const isUrlMatch = isMatchingUrl(response.url(), path);
			const isMethodMatch = method
				? response.request().method() === method
				: true;
			const isStatusMatch = isMatchingStatus(
				response.status(),
				expectedStatuses
			);

			return isUrlMatch && isMethodMatch && isStatusMatch;
		},
		{ timeout: timeoutMs }
	);
};
