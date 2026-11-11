import { spaces } from '@mms/mms-ui-library/theme';

export const popUpPaddings = {
	default: `${spaces.l}px`,
	projectsTab: `${spaces.s}px ${spaces.l}px ${spaces.l}px ${spaces.l}px`,
	hasControls: `${spaces.l}px ${spaces.l}px ${spaces.m}px ${spaces.l}px`,
} as const;

export const popUpBreakpoints = {
	mobile: 390,
	mobileL: 432,
	tabletXS: 600,
	tabletS: 611,
	tablet: 722,
	tabletL: 790,
	tabletXL: 832,
	desktopXS: 992,
} as const;

export const popUpSizes = {
	s: 400,
	m: 800,
	l: 960,
} as const;

export const POPUP_TABS_HEIGHT = 40;

export const POPUP_BUTTONS_CONTAINER_HEIGHT = 68;

export const MOBILE_POPUP_BUTTONS_CONTAINER_HEIGHT = 120;

export const POPUP_HEADER_HEIGHT = 40;

export const VERTICAL_PADDINGS = 86;

export const BORDER_HEIGHT = 2;
