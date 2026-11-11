import { Theme } from '@emotion/react';

import { popUpSizes } from '@/constants/popUp';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';

export const getPopUpStylesConfig = (theme: Theme, windowHeight: number) => ({
	container: {
		width: popUpSizes.m,
	},
	content: getAdaptivePopUpContentStylesConfig({
		theme,
		windowHeight,
		hasContols: true,
	}),
});
