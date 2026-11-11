import { Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { zIndexes } from '@/constants/zIndexes';
import { wikiBreakpoints } from '@/pages/Wiki/constants';

const BUTTON_WRAPPER_WIDTH = 100;

export const getHeadingToolbarStyles = (theme: Theme) =>
	({
		position: 'sticky',
		minHeight: 'auto',
		width: '-webkit-fill-available',
		padding: `${theme.spaces.m}px ${theme.spaces.m}px ${theme.spaces.xs}px`,
		margin: 0,
		backgroundColor: `${theme.palette['background-white']}`,
		borderRadius: `${BorderRadiuses.m}px ${BorderRadiuses.m}px 0 0`,
		borderBottom: `1px solid ${theme.palette['graphics-divider-1']}`,
		zIndex: zIndexes.headlingPlateEditor,
		justifyContent: 'space-between',
		flexWrap: 'wrap',
		gap: `${theme.spaces.m}px`,
	} as const);

export const ToolbarButtonsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.m,
	flex: 1,

	[theme.breakpoints.down(wikiBreakpoints.tablet)]: {
		gap: theme.spaces.s,
	},

	[theme.breakpoints.down(theme.breakpoints.tabletS)]: {
		gap: theme.spaces.xs,
	},
}));

export const HeadingToolbarButtons = styled('div')(({ theme }) => ({
	display: 'flex',
	paddingBottom: theme.spaces.m - theme.spaces.xs,
}));

export const ButtonWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'flex-end',
	width: BUTTON_WRAPPER_WIDTH,
	marginLeft: theme.spaces.l - theme.spaces.xs,
}));
