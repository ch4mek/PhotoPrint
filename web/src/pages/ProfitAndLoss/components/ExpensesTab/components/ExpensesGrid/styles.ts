import { css, Theme } from '@emotion/react';
import { ThemeMode } from '@mms/mms-ui-library/theme';

import {
	EXPENSE_CELL_CLASS,
	EXPENSES_GRID_EDIT_COLUMN_INDEX,
} from '../../constants';

import { overrideGridStylesWithEditColumn } from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
	EDIT_COLUMN_SUMMARY_CLASS,
} from '@/components/Grid/constants';
import { BASE_LINE_HEIGHT } from '@/constants/commonStyles';
import { zIndexes } from '@/constants/zIndexes';
import {
	getEvenRowBackgroundColor,
	getHoverBackgroundColor,
	getOddRowBackgroundColor,
} from '@/helpers/getColor';

import { EXPENSES_TOTAL_ROW_HEIGHT } from './constants';

export const overrideExpensesGridStyles =
	(templateColumns: string, isCompactGridView: boolean) => (theme: Theme) =>
		css`
			${overrideGridStylesWithEditColumn(
				true,
				EXPENSES_GRID_EDIT_COLUMN_INDEX,
				templateColumns
			)(theme).styles}

			.rdg-summary-row {
				background-color: ${theme.palette['background-base-2']};
			}

			.rdg-summary-row > .rdg-cell {
				position: sticky !important;
				z-index: ${zIndexes.summaryRow};
				background-color: ${theme.palette['background-base-2']};
				box-shadow: 0 1px 0 0 ${theme.palette['background-base-2']};
			}

			.rdg-summary-row > .${EDIT_COLUMN_SUMMARY_CLASS} {
				z-index: ${zIndexes.summaryRow + 1};
				background-color: ${theme.palette['background-base-2']};
				border-left: 1px solid ${theme.palette['graphics-divider-1']};
				position: sticky !important;
			}

			.rdg-summary-row > div[role='gridcell'] {
				&:before {
					display: none;
				}
			}

			.rdg-summary-row > div[role='gridcell']:not(:has(> *)) {
				border-right-color: transparent !important;
			}

			& > .rdg-row:is(.rdg-summary-row):hover > .rdg-cell {
				&:after {
					background: none !important;
					box-shadow: none !important;
				}

				background-color: ${theme.palette['background-base-2']};
			}

			.rdg-cell[role='columnheader'] div {
				background-color: transparent;
			}

			.rdg-cell[role='gridcell'] {
				line-height: ${BASE_LINE_HEIGHT};
				display: flex;
				align-items: flex-start;
				padding: 0;
				overflow: initial;
				justify-content: center;
				border-inline-end: none;
				&::before {
					content: '';
					position: absolute;
					width: 1px;
					height: 100%;
					top: 0;
					right: 0;
					background: ${theme.palette['graphics-divider-1']};
				}
			}

			.${EDIT_COLUMN_HEADER_CLASS} {
				&::before {
					left: 0;
					right: unset;
				}
			}

			.rdg-cell[role='gridcell'].${EXPENSE_CELL_CLASS} {
				padding: ${theme.spaces.xs}px ${theme.spaces.s}px;

				> div {
					display: flex;
					align-items: center;
				}
			}

			.rdg-cell[role='columnheader'] {
				z-index: ${zIndexes.header};

				&::before {
					content: '';
					position: absolute;
					width: 1px;
					height: 100%;
					top: 0;
					right: 0;
					background: ${theme.palette['graphics-input-stroke']};
				}
			}

			.rdg-cell[role='gridcell']:not([role='columnheader']):is(
					.${EDIT_COLUMN_CELL_CLASS}
				) {
				border-right: none;

				${!isCompactGridView &&
				css`
					position: relative !important;
				`}
			}

			${isCompactGridView &&
			css`
				.rdg-cell.${EDIT_COLUMN_HEADER_CLASS},
					.rdg-cell.${EDIT_COLUMN_CELL_CLASS},
					.rdg-cell.${EDIT_COLUMN_SUMMARY_CLASS} {
					position: sticky !important;
				}

				.rdg-cell.${EDIT_COLUMN_HEADER_CLASS} {
					z-index: ${zIndexes.header};
					background-color: ${theme.mode === ThemeMode.LIGHT
						? theme.palette['background-primary-1']
						: theme.palette['background-base-3']};
				}

				.rdg-row-odd > .${EDIT_COLUMN_CELL_CLASS} {
					background-color: ${getOddRowBackgroundColor(theme)} !important;
				}

				.rdg-row-even > .${EDIT_COLUMN_CELL_CLASS} {
					background-color: ${getEvenRowBackgroundColor(theme)} !important;
				}

				.rdg-row:hover > .${EDIT_COLUMN_CELL_CLASS} {
					background-color: ${getHoverBackgroundColor(theme)} !important;
				}

				.rdg-summary-row > .${EDIT_COLUMN_SUMMARY_CLASS} {
					z-index: ${zIndexes.summaryRow + 1};
					background-color: ${theme.palette['background-base-2']};
				}
			`}

			.${EDIT_COLUMN_CELL_CLASS} {
				z-index: ${zIndexes.editColumnCell};
				button {
					margin-top: ${theme.spaces.xs}px;
				}
			}

			.${EDIT_COLUMN_HEADER_CLASS} {
				z-index: ${zIndexes.sidebar + 1};
			}

			& > .rdg-row:hover .rdg-cell {
				background-color: inherit;
			}

			::-webkit-scrollbar-track:vertical {
				margin-bottom: ${EXPENSES_TOTAL_ROW_HEIGHT}px;
			}

			::-webkit-scrollbar:vertical {
				box-shadow: inset 0 -${EXPENSES_TOTAL_ROW_HEIGHT}px 0 0 ${theme.palette['background-base-2']};
			}
		`;
