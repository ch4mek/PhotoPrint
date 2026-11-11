import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { discountBreakpoints } from '@/constants/discounts';

export const DiscountsSectionWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'row',
	width: '100%',
	height: '100%',

	gap: theme.spaces.m,
	padding: theme.spaces.m,

	[theme.breakpoints.down(discountBreakpoints.desktopL)]: {
		flexDirection: 'column',
	},
}));

export const SectionWrapper = styled('div')(({ theme }) => ({
	backgroundColor: theme.palette['background-white'],
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderRadius: BorderRadiuses.m,
	height: '100%',
}));
