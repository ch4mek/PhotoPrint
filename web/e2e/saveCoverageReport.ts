import { promises as fs } from 'fs';
import * as path from 'path';

import type { Page, TestInfo } from '@playwright/test';

import { UnknownObject } from '@/types/common';

const DEFAULT_COVERAGE_DIRECTORY = '.nyc_output';
const DEFAULT_COVERAGE_VARIABLE_NAME = '__coverage__';
const MAX_FILENAME_SEGMENT_LENGTH = 120;

function sanitizeFilenameSegment(segment: string): string {
	return segment
		.replace(/[^\w.-]+/g, '_')
		.slice(0, MAX_FILENAME_SEGMENT_LENGTH);
}

function generateCoverageFilename(testInfo: TestInfo): string {
	const timestamp = Date.now();
	const processId = process.pid;
	const randomSuffix = crypto.randomUUID();

	const projectNameSegment = sanitizeFilenameSegment(testInfo.project.name);
	const testTitleSegment = sanitizeFilenameSegment(testInfo.title);

	return `coverage-e2e-${projectNameSegment}-${testTitleSegment}-${timestamp}-${processId}-${randomSuffix}.json`;
}

function normalizeCoverageData(
	rawCoverageData: Record<string, { path?: string }>
): UnknownObject {
	return Object.values(rawCoverageData).reduce<UnknownObject>(
		(normalizedCoverageData, fileCoverage) => {
			const originalPath = fileCoverage.path;

			if (typeof originalPath !== 'string') {
				return normalizedCoverageData;
			}
			const normalizedPath = path.normalize(originalPath);
			normalizedCoverageData[normalizedPath] = {
				...fileCoverage,
				path: normalizedPath,
			};

			return normalizedCoverageData;
		},
		{}
	);
}

export async function saveCoverageReport(
	page: Page,
	testInfo: TestInfo
): Promise<boolean> {
	const coverageData = await page.evaluate(
		(coverageVariableName: string) =>
			(window as any)[coverageVariableName] ?? null,
		DEFAULT_COVERAGE_VARIABLE_NAME
	);

	if (!coverageData) {
		return false;
	}

	const outputDirectory = path.resolve(
		process.cwd(),
		DEFAULT_COVERAGE_DIRECTORY
	);
	await fs.mkdir(outputDirectory, { recursive: true });

	const filename = generateCoverageFilename(testInfo);
	const filePath = path.join(outputDirectory, filename);
	await fs.writeFile(
		filePath,
		JSON.stringify(normalizeCoverageData(coverageData))
	);

	return true;
}
