import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { zIndexes } from '@/constants/zIndexes';

export const DiscountItemWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
	padding: theme.spaces.s,
	position: 'relative',

	borderRadius: BorderRadiuses.l2,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	backgroundColor: theme.palette['background-white'],

	cursor: 'pointer',
	transition: 'all .2s ease',

	'&:hover': {
		backgroundColor: theme.palette['hover-base-2'],
	},
}));

export const ControlItemRow = styled('div')(({ theme }) => ({
	flex: 1,

	display: 'flex',
	justifyContent: 'space-between',
	gap: theme.spaces.xs,
}));

export const TitleTypographyWrapper = styled('div')(({ theme }) => ({
	height: '100%',
	width: ' 100%',
	textOverflow: 'ellipsis',
	overflow: 'hidden',

	...theme.typography.headlines.small,
	color: theme.palette['text-black'],
}));

export const DiscountBadge = styled('div')(({ theme }) => ({
	padding: `${theme.spaces.xs}px ${theme.spaces.s}px`,
	borderRadius: BorderRadiuses.xs2,

	backgroundColor: theme.palette['graphics-accent'],
	color: theme.palette['text-white'],

	...theme.typography.body['m-400'],
}));

export const ControlIconWrapper = styled('div')(({ theme }) => ({
	position: 'absolute',
	top: theme.spaces.m,
	right: theme.spaces.m,
	zIndex: zIndexes.controlButtons,

	borderRadius: BorderRadiuses.xs2,
	backgroundColor: theme.palette['background-white'],
}));
