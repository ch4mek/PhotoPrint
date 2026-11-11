import { css, Theme } from '@emotion/react';
import { Column } from 'react-data-grid';

import { FeedbackRow } from '../../types';

import { overrideGridStylesWithEditColumn } from '@/components/Grid';
import {
	DYNAMIC_CELL_CLASS,
	EDIT_COLUMN_CELL_CLASS,
} from '@/components/Grid/constants';
import { FEEDBACK_TAG_COLOR_STYLES } from '@/constants/feedback';
import { TeamboardTableMode } from '@/types/teamboard';

import {
	FEEDBACK_SECTION_TABLE_WIDTHS_AND_ORDER,
	FEEDBACK_TAB_TABLE_WIDTHS_AND_ORDER,
} from './constants';

export const getFeedbackTableColumnWidth = (mode: TeamboardTableMode) =>
	mode === TeamboardTableMode.Section
		? FEEDBACK_SECTION_TABLE_WIDTHS_AND_ORDER
		: FEEDBACK_TAB_TABLE_WIDTHS_AND_ORDER;

const tabModeStyles = css`
	flex-grow: 1;
	min-height: 0;
`;

export const overrideFeedbackGridStyles =
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

			${tabModeStyles.styles}

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

				& .${DYNAMIC_CELL_CLASS} {
					height: fit-content;
					text-wrap: wrap !important;
					white-space: normal !important;
					overflow-wrap: break-word;
					text-overflow: initial;
				}
			}
		`;

export const createColumnsComparator =
	(columnsOrder: string[]) =>
	(a: Column<FeedbackRow, unknown>, b: Column<FeedbackRow, unknown>) =>
		columnsOrder.indexOf(a?.key) - columnsOrder.indexOf(b?.key);

export const getFeedbackTagStyles = (row: FeedbackRow) =>
	FEEDBACK_TAG_COLOR_STYLES[row.type];

export const shouldDisplayLink = (row: FeedbackRow) =>
	Boolean(row.conversationId);
