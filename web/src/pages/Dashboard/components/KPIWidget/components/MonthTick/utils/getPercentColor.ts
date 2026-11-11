import { Theme } from '@emotion/react';

import { KPI_COLOR_KEYS } from '../constants';

export function getPercentColor(color: string, theme: Theme): string {
	const colorKey = color as keyof typeof KPI_COLOR_KEYS;

	return theme.palette[KPI_COLOR_KEYS[colorKey]] ?? theme.palette['text-gray'];
}
