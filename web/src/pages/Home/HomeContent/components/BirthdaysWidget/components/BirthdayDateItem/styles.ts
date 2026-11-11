import styled from '@emotion/styled';

const DATE_WRAPPER_WIDTH = 48;
const DATE_WRAPPER_HEIGHT = 20;
const DATE_WRAPPER_BORDER_RADIUS = 42;
const DATE_WRAPPER_MARGIN_TOP = 10;
const USER_CARD_WIDTH = 206;

export const USER_CARD_CLASS_NAME = 'user-card';

export const Container = styled('li')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.m,
	paddingBlock: theme.spaces.s,
	borderBottom: `1px solid ${theme.palette['graphics-divider-1']}`,

	':last-of-type': {
		borderBottom: 'none',
	},
}));

export const UserCardsContainer = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.m,
	flexWrap: 'wrap',

	[`.${USER_CARD_CLASS_NAME}`]: {
		width: USER_CARD_WIDTH,
		flex: 'unset',
	},
}));

export const DateWrapper = styled('div')(({ theme }) => ({
	minWidth: DATE_WRAPPER_WIDTH,
	height: DATE_WRAPPER_HEIGHT,
	borderRadius: DATE_WRAPPER_BORDER_RADIUS,
	marginTop: DATE_WRAPPER_MARGIN_TOP,
	backgroundColor: theme.palette['background-base-2'],
	color: theme.palette['text-gray'],
	display: 'flex',
	justifyContent: 'center',
	alignItems: 'center',
}));
