import styled from '@emotion/styled';

import { ASSIGNMENT_FORM_MOBILE_BREAKPOINT } from '../../constants';

export const TransferListWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,

	[theme.breakpoints.down(ASSIGNMENT_FORM_MOBILE_BREAKPOINT)]: {
		flexDirection: 'column',
		alignItems: 'center',
	},
}));

export const ListContentWrapper = styled('div')(({ theme }) => ({
	flex: 1,
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
	width: '100%',
}));
