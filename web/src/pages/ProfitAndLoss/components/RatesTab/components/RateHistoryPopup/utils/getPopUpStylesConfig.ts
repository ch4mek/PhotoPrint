import { Theme } from '@emotion/react';

import { RATE_HISTORY_POPUP_WIDTH } from '../constants';

import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';

export const getPopUpStylesConfig = (theme: Theme, windowHeight: number) => ({
	container: {
		width: '100%',
		maxWidth: RATE_HISTORY_POPUP_WIDTH,
	},
	content: getAdaptivePopUpContentStylesConfig({
		theme,
		windowHeight,
	}),
});
