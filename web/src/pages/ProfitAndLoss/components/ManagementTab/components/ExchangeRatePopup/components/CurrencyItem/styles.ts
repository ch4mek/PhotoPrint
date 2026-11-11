import styled from '@emotion/styled';

import {
	CURRENCY_ITEM_MAX_HEIGHT,
	HORIZONTAL_LINE_WIDTH,
} from '../../constants';

import { BorderRadiuses } from '@/constants/borderRadiuses';

export const CurrencyItemWrapper = styled('div')(({ theme }) => ({
	maxHeight: CURRENCY_ITEM_MAX_HEIGHT,
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'center',
	gap: theme.spaces.s,
	padding: `${theme.spaces.s}px ${theme.spaces.m}px`,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderRadius: BorderRadiuses.l2,
}));

export const CurrencyItemInfo = styled('div')({
	display: 'flex',
	flexDirection: 'column',
});

export const ExchangeRateDisplay = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,
}));

export const HorizontalLine = styled('div')(({ theme }) => ({
	width: HORIZONTAL_LINE_WIDTH,
	height: '1px',
	backgroundColor: theme.palette['graphics-input-stroke'],
}));

export const RateValue = styled('span')(({ theme }) => ({
	display: 'inline-flex',
	alignItems: 'center',
	gap: theme.spaces.xs,
}));
