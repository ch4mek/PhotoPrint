import { Theme } from '@emotion/react';

import { popUpBreakpoints, popUpSizes } from '@/constants/popUp';
import { AbsenceTabs } from '@/forms/Absence/AbsenceForm/constants';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';

const MIDDLE_BREAKPOINT = 737;

const formBreakpoints = {
	s: 703,
	m: popUpSizes.s,
	l: 356,
};

export const getPopUpStylesConfig = (
	theme: Theme,
	activeTab: AbsenceTabs,
	windowHeight: number
) => ({
	container: {
		width: popUpSizes.m,

		[theme.breakpoints.down(popUpBreakpoints.tabletXL)]: {
			minWidth: formBreakpoints.s,
		},

		[theme.breakpoints.down(MIDDLE_BREAKPOINT)]: {
			minWidth: formBreakpoints.m,
		},

		[theme.breakpoints.down(popUpBreakpoints.mobileL)]: {
			minWidth: formBreakpoints.l,
		},
	},
	content: getAdaptivePopUpContentStylesConfig({
		hasContols: true,
		theme,
		hasTabs: !!activeTab,
		windowHeight,
		isFixPopUpMaxHeight: false,
	}),
});
