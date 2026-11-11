import { Theme } from '@emotion/react';
import { MARK_BG_COLOR, MARK_COLOR } from '@udecode/plate';

import {
	darkBackgroundColorPalette,
	darkTextColorPalette,
	lightBackgroundColorPalette,
	lightTextColorPalette,
} from '@/constants/plateEditor';
import { ThemeMode } from '@/theme';

export const getPalette = (
	theme: Theme,
	pluginType: typeof MARK_COLOR | typeof MARK_BG_COLOR
): string[] => {
	if (pluginType === MARK_COLOR) {
		return theme.mode === ThemeMode.DARK
			? darkTextColorPalette
			: lightTextColorPalette;
	}

	if (pluginType === MARK_BG_COLOR) {
		return theme.mode === ThemeMode.DARK
			? darkBackgroundColorPalette
			: lightBackgroundColorPalette;
	}

	return [];
};
