import styled from '@emotion/styled';
import {
	Scrollbar,
	ScrollbarProps,
} from '@mms/mms-ui-library/components/Scrollbar';

import { BOOKING_SMALL_MOBILE_SCREEN_WIDTH } from '../../constants';

export const Wrapper = styled('div')({
	overflow: 'hidden',
	flexGrow: 1,
	minHeight: 0,
});

export const CustomScrollbar = styled(Scrollbar)<ScrollbarProps>(
	({ theme }) => ({
		padding: `${theme.spaces.m}px`,
		paddingBottom: 0,
		height: '100%',
		display: 'flex',
		flexDirection: 'column',
		alignItems: 'center',

		[theme.breakpoints.down(BOOKING_SMALL_MOBILE_SCREEN_WIDTH)]: {
			alignItems: 'unset',
		},
	})
);
