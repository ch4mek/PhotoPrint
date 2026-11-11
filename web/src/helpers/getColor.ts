import { Theme } from '@emotion/react';
import { ThemeMode } from '@mms/mms-ui-library/theme';

import { TotalColumnColor } from '@/theme';

const HOVER_LIGHT_THEME_BORDER_COLOR = '#565A69';

export const getOddRowBackgroundColor = (theme: Theme) =>
	theme.palette['even-stroke-base-1'];

export const getEvenRowBackgroundColor = (theme: Theme) =>
	theme.palette['background-white'];

export const getTaskInfoColumnBackgroundColor = (theme: Theme) =>
	theme.palette['background-white'];

export const getTotalColumnBackgroundColor = (theme: Theme) =>
	theme.mode === ThemeMode.DARK
		? TotalColumnColor.DARK
		: TotalColumnColor.LIGHT;

export const getHoverBackgroundColor = (theme: Theme) =>
	theme.palette['hover-base-2'];

export const getTotalColumnHoverBackgroundColor = (theme: Theme) =>
	theme.palette['hover-base-2'];

export const getHoverBorderColor = (theme: Theme) =>
	theme.mode === ThemeMode.DARK
		? theme.palette['graphics-divider-1']
		: HOVER_LIGHT_THEME_BORDER_COLOR;

export const createBorderColor = (theme: Theme) =>
	theme.palette['graphics-input-stroke'];

export const createActiveBorderColor = (theme: Theme) =>
	theme.palette['graphics-divider-1'];

export const createDefaultBorderColor = (theme: Theme) =>
	theme.palette['graphics-divider-2'];

export const createTransparentBorderColor = () => `transparent`;
