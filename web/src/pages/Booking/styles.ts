import styled from '@emotion/styled';
import {
	Scrollbar,
	ScrollbarProps,
} from '@mms/mms-ui-library/components/Scrollbar';

import { BOOKING_TABS_CHANGE_SCREEN_WIDTH } from './constants';

const TABS_HEIGHT = 37;
const PAGE_MIN_HEIGHT = 740;

export const BookingPageWrapper = styled(Scrollbar)(
	() =>
		({
			height: '100%',

			'::-webkit-scrollbar-track': {
				borderTop: `${TABS_HEIGHT}px solid transparent`,
			},
			'::-webkit-scrollbar-track-piece:vertical': {
				marginTop: TABS_HEIGHT,
			},
		} as const)
);

export const BookingContentWrapper = styled('div')(
	({ theme }) =>
		({
			position: 'relative',
			display: 'flex',
			flexDirection: 'column',
			height: '100%',

			[theme.breakpoints.down(BOOKING_TABS_CHANGE_SCREEN_WIDTH)]: {
				minHeight: PAGE_MIN_HEIGHT,
			},
		} as const)
);

export const BookingWrapper = styled(Scrollbar)<ScrollbarProps>(
	({ theme }) => ({
		height: '100%',
		display: 'grid',
		gridTemplateColumns: 'auto 1fr',
		gap: theme.spaces.m,
		padding: theme.spaces.m,

		[theme.breakpoints.down(BOOKING_TABS_CHANGE_SCREEN_WIDTH)]: {
			gridTemplateColumns: '1fr',
			gridTemplateRows: '1fr',
			gap: theme.spaces.l,
		},
	})
);
