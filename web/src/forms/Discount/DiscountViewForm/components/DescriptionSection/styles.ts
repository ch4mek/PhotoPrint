import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { scrollbarWidths } from '@/constants/commonStyles';
import { MIDDLE_BREAKPOINT } from '@/forms/Discount/DiscountViewForm/constants';

export const DescriptionSectionWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,

	flex: 1,
	paddingBottom: theme.spaces.l,

	[theme.breakpoints.up(MIDDLE_BREAKPOINT)]: {
		padding: `0 ${theme.spaces.l}px ${theme.spaces.l}px 0`,

		...getScrollbarStyles(theme, scrollbarWidths.m, 'vertical'),
	},
}));

export const CitiesSection = styled('div')(({ theme }) => ({
	color: theme.palette['text-black'],
	whiteSpace: 'normal',
	wordBreak: 'break-word',
	flex: 1,

	...theme.typography.body['m-400'],
}));

export const DescriptionWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,

	flex: 1,
	color: theme.palette['text-black'],
}));

export const DetailsInfoWrapper = styled('div')<{ alignStart?: boolean }>(
	({ theme, alignStart }) => ({
		display: 'flex',
		alignItems: alignStart ? 'flex-start' : 'center',
		gap: theme.spaces.s,
	})
);

export const DiscountBadge = styled('div')(({ theme }) => ({
	padding: `${theme.spaces.xs}px ${theme.spaces.s}px`,
	borderRadius: BorderRadiuses.xs2,

	backgroundColor: theme.palette['graphics-accent'],
	color: theme.palette['text-white'],

	...theme.typography.body['m-400'],
}));
