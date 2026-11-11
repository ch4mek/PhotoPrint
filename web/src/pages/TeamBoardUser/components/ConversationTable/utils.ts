import { css, Theme } from '@emotion/react';

import { alternateTabTableRowsColor } from '../../constants';
import { ConversationRow } from '../../types';

import { overrideGridStylesWithEditColumn } from '@/components/Grid';
import { EDIT_COLUMN_CELL_CLASS } from '@/components/Grid/constants';
import { FEEDBACK_TAG_COLOR_STYLES } from '@/constants/feedback';
import { TeamboardTableMode } from '@/types/teamboard';

import {
	CONVERSATION_SECTION_EDIT_COLUMN_INDEX,
	CONVERSATION_SECTION_TABLE_WIDTHS,
	CONVERSATION_TAB_EDIT_COLUMN_INDEX,
	CONVERSATION_TAB_TABLE_WIDTHS,
} from './constants';

export const getConversationTableColumnWidth = (mode: TeamboardTableMode) =>
	mode === TeamboardTableMode.Section
		? CONVERSATION_SECTION_TABLE_WIDTHS
		: CONVERSATION_TAB_TABLE_WIDTHS;

export const getConversationEditColumnIndex = (mode: TeamboardTableMode) =>
	mode === TeamboardTableMode.Section
		? CONVERSATION_SECTION_EDIT_COLUMN_INDEX
		: CONVERSATION_TAB_EDIT_COLUMN_INDEX;

export const getConversationResultTagStyles = ({
	conversationResult,
}: ConversationRow) =>
	conversationResult ? FEEDBACK_TAG_COLOR_STYLES[conversationResult] : {};

const tabModeStyles = (theme: Theme) => css`
	flex-grow: 1;
	min-height: 0;
	${alternateTabTableRowsColor(theme).styles};
`;

export const overrideConversationGridStyles =
	(
		isPaginationShown: boolean,
		mode: TeamboardTableMode,
		columnIndex?: number,
		templateColumns?: string
	) =>
	(theme: Theme) =>
		css`
			${overrideGridStylesWithEditColumn(
				isPaginationShown,
				columnIndex,
				templateColumns
			)(theme).styles}

			${mode === TeamboardTableMode.Tab && tabModeStyles(theme).styles}

			& .rdg-cell.${EDIT_COLUMN_CELL_CLASS} {
				padding-top: ${theme.spaces.xs}px;
				display: flex;
				justify-content: center;
			}

			.rdg-cell {
				display: flex;
				align-items: flex-start;
				padding-top: ${theme.spaces.xs}px;
				line-height: ${theme.typography.body['m-400'].lineHeight};
			}
		`;
