import { defineConfig, loadEnv } from 'vite';
import react from '@vitejs/plugin-react';
import path from 'path';
import { federation } from '@module-federation/vite';
import { analyzer } from 'vite-bundle-analyzer';
import istanbul from 'vite-plugin-istanbul';
import obfuscatorPlugin from 'vite-plugin-javascript-obfuscator';
import { stripTestAttributesPlugin } from './src/plugins/VitePlugins';

export default defineConfig(({ mode }) => {
	const env = loadEnv(mode, process.cwd(), '');
	const isCoverageEnabled = process.env.COVERAGE === 'true';
	const shouldStripTestAttributes = env.VITE_ENV === 'production';
	const isDevMode = process.env.ENV === 'development';

	return {
		build: {
			outDir: 'build',
			minify: !isDevMode,
			sourcemap: isDevMode,
		},
		resolve: {
			alias: {
				'@': path.resolve(__dirname, 'src'),
				'@/theme': path.resolve(__dirname, 'src/theme'),
				'@/forms': path.resolve(__dirname, 'src/forms'),
				'@/fields': path.resolve(__dirname, 'src/fields'),
				'@/components': path.resolve(__dirname, 'src/components'),
				'@/pages': path.resolve(__dirname, 'src/pages'),
				'@/context': path.resolve(__dirname, 'src/context'),
				'@/constants': path.resolve(__dirname, 'src/constants'),
				'@/layouts': path.resolve(__dirname, 'src/layouts'),
				'@/assets': path.resolve(__dirname, 'src/assets'),
				'@/helpers': path.resolve(__dirname, 'src/helpers'),
				'@/routes': path.resolve(__dirname, 'src/routes'),
				'@/services': path.resolve(__dirname, 'src/services'),
				'@/notifications': path.resolve(__dirname, 'src/notifications'),
				'@/lib': path.resolve(__dirname, 'src/lib'),
				'@/stores': path.resolve(__dirname, 'src/stores'),
				'@/i18n': path.resolve(__dirname, 'src/i18n'),
			},
		},
		plugins: [
			react(),
			...(shouldStripTestAttributes ? [stripTestAttributesPlugin()] : []),
			obfuscatorPlugin({
				apply: 'build',
				exclude: [/node_modules/],
				options: {
					compact: true,
					splitStrings: true,
					stringArray: true,
					stringArrayEncoding: ['rc4'],
					ignoreImports: true,
					controlFlowFlattening: false,
					deadCodeInjection: false,
					identifierNamesGenerator: 'hexadecimal',
					numbersToExpressions: false,
					renameGlobals: true,
					selfDefending: false,
					simplify: true,
					unicodeEscapeSequence: false,
				},
			}),
			...[
				isCoverageEnabled &&
					istanbul({
						include: 'src/**/*.{ts,tsx,js,jsx}',
						exclude: [
							'node_modules',
							'src/**/*.test.{ts,tsx,js,jsx}',
							'src/**/styles.ts',
							'src/**/styled.ts',
						],
						extension: ['.js', '.jsx', '.ts', '.tsx'],
						requireEnv: false,
					}),
			].filter(Boolean),
			analyzer({
				analyzerMode:
					process.env?.ANALYZER_MODE === 'server' ? 'server' : 'static',
			}),
			federation({
				name: 'mms_host',
				filename: 'remoteEntry-[hash].js',
				remotes: {},
				shared: {
					react: {
						singleton: true,
						strictVersion: false,
						requiredVersion: '^19.0.0',
					},
					'react-dom': {
						singleton: true,
						strictVersion: false,
						requiredVersion: '^19.0.0',
					},
					'@emotion/react': {
						singleton: true,
						strictVersion: false,
						version: '11.14.0',
					},
					'@tanstack/react-query': {
						singleton: true,
						strictVersion: false,
					},
					'@mui/material': {
						singleton: true,
						strictVersion: false,
						version: '5.16.14',
					},
				},
			}),
		],
		preview: {
			port: 3000,
		},
		server: {
			port: 3000,
		},
	};
});
