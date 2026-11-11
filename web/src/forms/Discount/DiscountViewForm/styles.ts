import styled from '@emotion/styled';

import { MIDDLE_BREAKPOINT } from '@/forms/Discount/DiscountViewForm/constants';

export const FormContent = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'row',
	gap: theme.spaces.l,

	height: '100%',

	[theme.breakpoints.down(MIDDLE_BREAKPOINT)]: {
		flexDirection: 'column',
	},
}));
