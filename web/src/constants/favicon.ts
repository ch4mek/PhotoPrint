import { FaviconConfig } from '@/types/favicon';

import { Routes } from './routes';

export const FAVICON_CONFIGS: ReadonlyArray<FaviconConfig> = [
	{
		rel: 'icon',
		filename: 'favicon-96x96.png',
		sizes: '96x96',
		type: 'image/png',
	},
	{ rel: 'icon', filename: 'favicon.svg', type: 'image/svg+xml' },
	{ rel: 'shortcut icon', filename: 'favicon.ico' },
	{
		rel: 'apple-touch-icon',
		filename: 'apple-touch-icon.png',
		sizes: '180x180',
	},
	{ rel: 'manifest', filename: 'site.webmanifest' },
];

export const ROUTE_TO_ICON_FOLDER_MAP: Readonly<Record<string, string>> = {
	[Routes.discounts]: 'discounts',
	[Routes.recruitment]: 'recruitment',
	[Routes.wiki]: 'wiki',
};

export const DEFAULT_ICON_FOLDER = 'modsen';
