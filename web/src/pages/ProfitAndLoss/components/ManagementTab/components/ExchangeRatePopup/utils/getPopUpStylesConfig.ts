import { Theme } from '@emotion/react';

import {
	EXCHANGE_RATE_POPUP_MAX_HEIGHT,
	EXCHANGE_RATE_POPUP_MAX_WIDTH,
} from '../constants';

import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';

export const getPopUpStylesConfig = (theme: Theme, windowHeight: number) => {
	const adaptiveContentStyles = getAdaptivePopUpContentStylesConfig({
		theme,
		hasContols: false,
		windowHeight,
		isFixPopUpMaxHeight: true,
	});

	return {
		container: {
			width: EXCHANGE_RATE_POPUP_MAX_WIDTH,
		},
		content: {
			...adaptiveContentStyles,
			maxHeight: EXCHANGE_RATE_POPUP_MAX_HEIGHT,
		},
	};
};
