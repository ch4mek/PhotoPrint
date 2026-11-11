import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const PERCENT_INPUT_WIDTH = 140;
const PERCENT_ITEM_HEIGHT = 40;
const ERROR_TEXT_CLASSNAME = '.error-text';

export const KPITabFormWrapper = styled('form')(({ theme }) => ({
	padding: theme.spaces.m,
	marginTop: theme.spaces.m,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderRadius: BorderRadiuses.xl,
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
	width: 'fit-content',
	backgroundColor: theme.palette['background-white'],
	color: theme.palette['text-black'],
}));

export const KPIPercentItem = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'space-between',
	gap: theme.spaces.m,
	height: PERCENT_ITEM_HEIGHT,
}));

export const KPIColorPercentInputWrapper = styled('div')(({ theme }) => ({
	maxWidth: PERCENT_INPUT_WIDTH,
	minWidth: PERCENT_INPUT_WIDTH,
	[ERROR_TEXT_CLASSNAME]: {
		backgroundColor: theme.palette['background-white'],
	},
}));
