import { defineConfig, devices } from '@playwright/test';
import { loadEnv } from 'vite';

const env = loadEnv(process.env.NODE_ENV ?? 'development', process.cwd(), '');
process.env = { ...process.env, ...env };

export default defineConfig({
	testDir: './e2e',
	fullyParallel: false,
	forbidOnly: !!process.env.CI,
	reporter: [['list'], ['junit', { outputFile: '.nyc_output/e2e-junit.xml' }]],
	retries: 3,
	use: {
		baseURL: 'http://localhost:3000',
		trace: 'on-first-retry',
	},
	projects: [
		{
			name: 'chromium',
			use: { ...devices['Desktop Chrome'] },
		},
	],
	webServer: {
		command: 'yarn start',
		url: 'http://localhost:3000',
		reuseExistingServer: !process.env.CI && process.env.COVERAGE !== 'true',
	},
});
