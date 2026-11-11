import { css } from '@emotion/react';
import styled from '@emotion/styled';

import { TabsSectionProps } from './types';

const TABS_SECTION_MIN_HEIGHT = 320;

export const ContentForm = styled('div')(
	({ theme }) => css`
		display: flex;
		flex-direction: column;
		flex: auto;
		gap: ${theme.spaces.m}px;
		width: 100%;
	`
);

export const ContentHeader = styled('div')<{
	hasUpcomingReview?: boolean;
}>(({ theme, hasUpcomingReview = true }) => ({
	display: 'flex',
	justifyContent: hasUpcomingReview ? 'space-between' : 'flex-end',
	gap: `${theme.spaces.m}px`,

	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		flexDirection: 'column-reverse',
	},
}));

export const TabsSection = styled('div')<TabsSectionProps>(
	({ minHeight, maxHeight }) => ({
		width: '100%',
		flexGrow: 1,
		minHeight: minHeight || TABS_SECTION_MIN_HEIGHT,
		height: '100%',
		maxHeight: maxHeight || '100%',
	})
);
