import styled from '@emotion/styled';

import { popUpBreakpoints } from '@/constants/popUp';

const COLS = 3;

export const FormRow = styled('div')(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: `repeat(${COLS}, 1fr)`,
	width: '100%',
	gap: theme.spaces.s,

	[theme.breakpoints.down(popUpBreakpoints.tabletL)]: {
		gridTemplateColumns: `auto`,
		gap: theme.spaces.l,
	},
}));
