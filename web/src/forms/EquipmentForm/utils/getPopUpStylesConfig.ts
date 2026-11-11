import { Theme } from '@emotion/react';

import { EquipmentTabs, POPUP_TABLET_WIDTH } from '../constants';

import { popUpBreakpoints, popUpSizes } from '@/constants/popUp';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';

export const getPopUpStylesConfig = (
	theme: Theme,
	activeTab: EquipmentTabs,
	windowHeight: number
) => ({
	container: {
		width: popUpSizes.l,

		[theme.breakpoints.down(popUpBreakpoints.desktopXS)]: {
			width: POPUP_TABLET_WIDTH,
		},

		[theme.breakpoints.down(popUpBreakpoints.tabletL)]: {
			width: popUpSizes.s,
		},
	},
	content: getAdaptivePopUpContentStylesConfig({
		hasContols: activeTab === EquipmentTabs.EQUIPMENT,
		theme,
		hasTabs: true,
		windowHeight,
	}),
});
