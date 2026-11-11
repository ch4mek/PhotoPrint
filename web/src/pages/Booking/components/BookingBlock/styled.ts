import styled from '@emotion/styled';

import {
	BOOKING_TABS_CHANGE_SCREEN_WIDTH,
	BOOKING_SMALL_MOBILE_SCREEN_WIDTH,
} from '../../constants';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const BOOKING_WRAPPER_WIDTH = 342;

export const BookingsBlockContent = styled('div')(({ theme }) => ({
	border: `1px solid ${theme.palette['graphics-input-stroke']}`,
	borderTop: 'none',
	backgroundColor: theme.palette['background-white'],
	borderRadius: `0 0 ${BorderRadiuses.m}px ${BorderRadiuses.m}px`,
	overflow: 'hidden',
	width: '100%',
	flexGrow: 1,
	minHeight: 0,
	display: 'flex',
	flexDirection: 'column',
}));

export const BookingsBody = styled('div')({
	height: '100%',
	display: 'flex',
	flexDirection: 'column',
});

export const BookingBlockWrapper = styled('div')(({ theme }) => ({
	width: BOOKING_WRAPPER_WIDTH,
	height: '100%',
	display: 'flex',
	flexDirection: 'column',

	overflow: 'hidden',

	[theme.breakpoints.down(BOOKING_TABS_CHANGE_SCREEN_WIDTH)]: {
		justifySelf: 'center',
	},

	[theme.breakpoints.down(BOOKING_SMALL_MOBILE_SCREEN_WIDTH)]: {
		width: '100%',
		padding: `0 ${theme.spaces.xs}px 0 ${theme.spaces.xs}px`,
	},
}));
