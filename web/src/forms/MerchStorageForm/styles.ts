import styled from '@emotion/styled';

import { popUpBreakpoints } from '@/constants/popUp';

export const ThreeInputsRowWrapper = styled('div')(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: `repeat(3, 1fr)`,
	gap: theme.spaces.s,
	alignItems: 'center',

	[theme.breakpoints.down(popUpBreakpoints.tabletXL)]: {
		gridTemplateColumns: 'repeat(2, 1fr)',
		rowGap: theme.spaces.l,

		'& > :nth-child(3)': {
			gridColumn: '1 / -1',
		},
	},
}));

export const SizesSection = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
	width: '100%',
}));
