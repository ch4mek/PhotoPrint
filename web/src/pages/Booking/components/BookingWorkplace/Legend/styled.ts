import styled from '@emotion/styled';

import { BOOKING_MOBILE_SCREEN_WIDTH } from '@/pages/Booking/constants';

export const ControlsWrapper = styled('div')<{ hasLegend: boolean }>(
	({ theme, hasLegend }) => ({
		display: 'flex',
		justifyContent: hasLegend ? 'space-between' : 'flex-end',
		width: '100%',
		alignItems: 'center',
		flexShrink: 0,
		padding: `${theme.spaces.s}px ${theme.spaces.s}px`,
	})
);

export const LegendWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,

	[theme.breakpoints.down(BOOKING_MOBILE_SCREEN_WIDTH)]: {
		marginTop: 0,
	},
}));

export const ColorTooltipTextWrapper = styled('div')(
	() =>
		({
			'&:first-letter': {
				textTransform: 'uppercase',
			},
		} as const)
);
