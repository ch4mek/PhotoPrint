import styled from '@emotion/styled';

import { BOOKING_SMALL_MOBILE_SCREEN_WIDTH } from '../../constants';

export const BookingFormWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	alignItems: 'center',
	gap: theme.spaces.m,
	height: '100%',

	[theme.breakpoints.down(BOOKING_SMALL_MOBILE_SCREEN_WIDTH)]: {
		width: '100%',
	},
}));
