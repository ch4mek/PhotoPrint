import { Theme } from '@emotion/react';

import { popUpBreakpoints, popUpPaddings, popUpSizes } from '@/constants/popUp';
import { PayloadFormTabs } from '@/forms/PayloadForm/constants';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';

export const getPopUpStylesConfig = (
	theme: Theme,
	activeTab: PayloadFormTabs,
	windowHeight: number
) => ({
	container: {
		width: popUpSizes.m,

		[theme.breakpoints.down(popUpBreakpoints.tablet)]: {
			width: popUpSizes.s,
		},
	},
	content: {
		...getAdaptivePopUpContentStylesConfig({
			theme,
			hasTabs: true,
			hasContols: true,
			windowHeight,
		}),

		padding:
			activeTab === PayloadFormTabs.Details
				? popUpPaddings.hasControls
				: popUpPaddings.projectsTab,
	},
});
