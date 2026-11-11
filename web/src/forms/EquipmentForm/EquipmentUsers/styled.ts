import styled from '@emotion/styled';

import {
	USER_TAB_HEIGHT,
	USER_TAB_MOBILE_HEIGHT,
	USER_TAB_TABLET_HEIGHT,
} from '../constants';

import { popUpBreakpoints } from '@/constants/popUp';

export const Wrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.l,
	position: 'relative',
	zIndex: 0,
	height: USER_TAB_HEIGHT,

	[theme.breakpoints.down(popUpBreakpoints.desktopXS)]: {
		height: USER_TAB_TABLET_HEIGHT,
		flexDirection: 'column',
	},

	[theme.breakpoints.down(popUpBreakpoints.tabletL)]: {
		height: USER_TAB_MOBILE_HEIGHT,
	},
}));
