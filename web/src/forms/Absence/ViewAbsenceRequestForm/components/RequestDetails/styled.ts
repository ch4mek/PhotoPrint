import styled from '@emotion/styled';

import { popUpBreakpoints } from '@/constants/popUp';

export const RequestDetailsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
}));

export const CalendarInfoWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
}));

export const CountValuesWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.m,

	[theme.breakpoints.down(popUpBreakpoints.tabletXS)]: {
		flexWrap: 'wrap',
		gap: theme.spaces.s,
	},
}));
