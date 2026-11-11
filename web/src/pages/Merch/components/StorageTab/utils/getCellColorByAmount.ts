import { Theme } from '@emotion/react';

import { ThemeMode } from '@/theme';

import { checkIfStatusRunningOut } from './checkIfStatusRunningOut';

const RED_WHITE_PROPORTION = 90;
const ORANGE_WHITE_PROPORTION = 85;
const DARK_THEME_PROPORTION = 60;

const getColorMix = (theme: Theme, color: string, whitePercentage: number) =>
	`color-mix(in srgb, ${
		theme.mode === ThemeMode.LIGHT
			? theme.palette['graphics-white']
			: theme.palette['even-stroke-base-1']
	} ${whitePercentage}%, ${color})`;

export const getCellColorByAmount = (
	theme: Theme,
	amount?: number,
	status?: string
) => {
	if (checkIfStatusRunningOut(status)) {
		return amount === 0
			? getColorMix(
					theme,
					theme.palette['graphics-error'],
					theme.mode === ThemeMode.LIGHT
						? RED_WHITE_PROPORTION
						: DARK_THEME_PROPORTION
			  )
			: getColorMix(
					theme,
					theme.palette['graphics-yellow'],
					theme.mode === ThemeMode.LIGHT
						? ORANGE_WHITE_PROPORTION
						: DARK_THEME_PROPORTION
			  );
	}

	return undefined;
};
