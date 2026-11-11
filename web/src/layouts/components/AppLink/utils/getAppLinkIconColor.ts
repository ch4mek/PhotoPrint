import { Theme } from '@emotion/react';
import { lightTheme } from '@mms/mms-ui-library/theme';

import { MENU_DARK_COLOR, ThemeMode } from '@/theme';

export const getAppLinkIconColor = (
	hasSpecificColor: boolean,
	theme: Theme
) => {
	if (hasSpecificColor) {
		return theme.mode === ThemeMode.DARK
			? MENU_DARK_COLOR
			: lightTheme.palette['background-primary-3'];
	}

	return undefined;
};
