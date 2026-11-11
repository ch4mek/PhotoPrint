import styled from '@emotion/styled';

import { PAYLOAD_MOBILE_BREAKPOINT } from '../../contants';

export const PayloadHeaderWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
}));

export const HeaderSection = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	justifyContent: 'space-between',

	[theme.breakpoints.down(PAYLOAD_MOBILE_BREAKPOINT)]: {
		flexDirection: 'row',
		alignItems: 'center',
	},
}));
