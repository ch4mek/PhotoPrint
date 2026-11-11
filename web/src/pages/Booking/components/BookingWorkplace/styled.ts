import styled from '@emotion/styled';

import { BOOKING_TABS_CHANGE_SCREEN_WIDTH } from '../../constants';

import { BorderRadiuses } from '@/constants/borderRadiuses';

import { bookingGridAreas } from './constants';

const { titleArea } = bookingGridAreas;

export const BookingWorkplaceWrapper = styled('div')(({ theme }) => ({
	height: '100%',
	minHeight: '100%',
	width: '100%',
	maxWidth: '100%',
	minWidth: '100%',
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
	backgroundColor: theme.palette['background-white'],
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderRadius: BorderRadiuses.m,

	[theme.breakpoints.down(BOOKING_TABS_CHANGE_SCREEN_WIDTH)]: {
		justifySelf: 'center',
	},
}));

export const TitleWrapper = styled('div')(() => ({
	gridArea: titleArea,
}));
