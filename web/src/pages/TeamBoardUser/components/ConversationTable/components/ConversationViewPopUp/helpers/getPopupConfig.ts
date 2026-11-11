import { Theme } from '@emotion/react';

import { CONVERSATION_VIEW_POPUP_WIDTH } from '../constants';

import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';

export const getPopupConfig = (theme: Theme, windowHeight: number) => ({
	container: {
		width: CONVERSATION_VIEW_POPUP_WIDTH,
	},
	content: getAdaptivePopUpContentStylesConfig({
		theme,
		windowHeight,
		hasContols: false,
	}),
});
