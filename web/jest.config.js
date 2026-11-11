const config = {
	verbose: true,
	testEnvironmentOptions: {
		url: 'http://localhost/',
	},
	extensionsToTreatAsEsm: ['.ts', '.tsx'],
	moduleNameMapper: {
		'@/(.*)$': '<rootDir>/src/$1',
		'^(\\.{1,2}/.*)\\.js$': '$1',
	},
	reporters: [
		'default',
		[
			'jest-junit',
			{ outputDirectory: '.nyc_output', outputName: 'jest-junit.xml' },
		],
	],
	moduleFileExtensions: ['ts', 'tsx', 'js', 'jsx'],
	transform: {
		'^.+\\.[t|j]sx?$': 'babel-jest',
		'\\.(jpg|jpeg|png|gif|eot|otf|webp|svg|ttf|woff|woff2|mp4|webm|wav|mp3|m4a|aac|oga)$':
			'<rootDir>/fileTransformer.js',
	},
	collectCoverage: true,
	collectCoverageFrom: ['src/**/*.{ts,tsx}', '!**/styles.ts', '!**/styled.ts'],
	coverageReporters: ['json', 'text', 'lcov'],
	coverageDirectory: '.nyc_output',
	testMatch: ['<rootDir>/src/**/*.test.{ts,tsx}'],
	transformIgnorePatterns: [
		'node_modules/(?!(@mms/mms-ui-library|@udecode|react-dnd|react-dnd-html5-backend|dnd-core|@react-dnd)/)',
	],
	resetModules: true,
	maxWorkers: process.env.CI ? '30%' : '100%',
	workerIdleMemoryLimit: '512MB',
};

export default config;
