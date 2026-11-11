import { Theme } from '@emotion/react';

import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';

import { POPUP_MAX_WIDTH } from '../constants';

export const getPopUpStylesConfig = (theme: Theme, windowHeight: number) => ({
	container: {
		width: '100%',
		maxWidth: POPUP_MAX_WIDTH,
	},
	content: getAdaptivePopUpContentStylesConfig({
		theme,
		hasContols: true,
		windowHeight,
	}),
});
