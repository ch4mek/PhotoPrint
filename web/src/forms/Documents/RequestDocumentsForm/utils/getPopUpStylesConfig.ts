import { Theme } from '@emotion/react';

import { popUpBreakpoints, popUpSizes } from '@/constants/popUp';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';

export const getPopUpStylesConfig = (theme: Theme, windowHeight: number) => ({
	container: {
		width: popUpSizes.m,

		[theme.breakpoints.down(popUpBreakpoints.tabletXS)]: {
			width: popUpSizes.s,
		},
	},
	content: getAdaptivePopUpContentStylesConfig({
		theme,
		hasContols: true,
		windowHeight,
	}),
});
