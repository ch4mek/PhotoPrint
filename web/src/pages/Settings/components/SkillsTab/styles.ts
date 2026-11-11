import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';

import { SettingsPageGridWrapper } from '../../styles';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { scrollbarWidths } from '@/constants/commonStyles';

import { DEPARTMENTS_LIST_WIDTH, SKILL_LIST_WIDTH } from './constants';

const ListContainer = styled('div')(
	({ theme }) => css`
		display: grid;
		border: 1px solid ${theme.palette['graphics-divider-1']};
		border-radius: ${BorderRadiuses.xl}px;
		background-color: ${theme.palette['background-base-2']};
		height: 100%;
		padding: ${theme.spaces.m}px;
		grid-auto-rows: 100%;
	`
);

export const LeftListContainer = styled(ListContainer)(
	({ theme }) => css`
		grid-template-columns: ${DEPARTMENTS_LIST_WIDTH}px ${SKILL_LIST_WIDTH}px ${SKILL_LIST_WIDTH}px;
		grid-auto-rows: 100%;
		column-gap: ${theme.spaces.m}px;
	`
);

export const RightListContainer = styled(ListContainer)(
	() => css`
		grid-template-columns: ${SKILL_LIST_WIDTH}px;
	`
);

export const SkillsListsWrapper = styled(SettingsPageGridWrapper)(
	({ theme }) => css`
		display: grid;
		grid-auto-rows: 100%;
		grid-template-columns: min-content min-content;
		grid-column-gap: ${theme.spaces.xl2}px;
		flex-grow: 1;
		min-height: 0;
		height: 100%;
		width: 100%;

		${getScrollbarStyles(theme, scrollbarWidths.m, 'horizontal')};

		::-webkit-scrollbar-track {
			margin-left: ${theme.spaces.l}px;
			margin-right: ${theme.spaces.l}px;
		}

		${theme.breakpoints.down(theme.breakpoints.desktopXS)} {
			::-webkit-scrollbar-track {
				margin-left: ${theme.spaces.m}px;
				margin-right: ${theme.spaces.m}px;
			}
		}
	`
);
