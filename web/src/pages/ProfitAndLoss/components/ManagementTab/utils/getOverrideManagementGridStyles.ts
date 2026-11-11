import { css, Theme } from '@emotion/react';

import {
	getManagementGridColumnsWidths,
	MANAGEMENT_SUMMARY_LEFT_CELL_CLASS,
	MANAGEMENT_SUMMARY_LEFT_COLUMNS_COUNT,
	MANAGEMENT_SUMMARY_LEFT_SPAN_END_COLUMN,
	MANAGEMENT_SUMMARY_MIDDLE_COLUMNS_END,
	MANAGEMENT_SUMMARY_VALUE_COLUMNS_END,
	MANAGEMENT_TOTAL_SUMMARY_ROW_HEIGHT,
} from '../constants';

import { zIndexes } from '@/constants/zIndexes';
import { getOverrideRatesGridStyles } from '@/pages/ProfitAndLoss/utils/getOverrideRatesGridStyles';

const EDIT_COLUMN_OFFSET = 2;

const summaryZIndex = {
	cell: zIndexes.summaryRow,
	left: zIndexes.summaryRow + 1,
	edit: zIndexes.summaryRow + 2,
	value: zIndexes.summaryRow + 3,
} as const;

export const getOverrideManagementGridStyles =
	(isDesktopWidth: boolean, rowsLength: number) => (gridTheme: Theme) => {
		const columnsWidths = getManagementGridColumnsWidths(isDesktopWidth);
		const baseStyles = getOverrideRatesGridStyles(
			columnsWidths,
			rowsLength
		)(gridTheme);

		const summaryRowBackground = gridTheme.palette['background-base-2'];
		const visibleColumnsCount = Object.keys(columnsWidths).length;
		const summaryToColumn = visibleColumnsCount + EDIT_COLUMN_OFFSET;
		const summaryValueColumnsStart = MANAGEMENT_SUMMARY_MIDDLE_COLUMNS_END + 1;
		const summaryEditFromColumn = MANAGEMENT_SUMMARY_VALUE_COLUMNS_END + 1;

		return css`
			${baseStyles}

			${rowsLength > 0 &&
			css`
				.rdg {
					box-shadow: inset 0 -${MANAGEMENT_TOTAL_SUMMARY_ROW_HEIGHT}px 0 0 ${summaryRowBackground};
				}

				::-webkit-scrollbar:vertical {
					box-shadow: inset 0 -${MANAGEMENT_TOTAL_SUMMARY_ROW_HEIGHT}px 0 0 ${summaryRowBackground};
				}
			`}

			.rdg-summary-row,
			.rdg-bottom-summary-row {
				.rdg-cell {
					position: sticky !important;
					z-index: ${summaryZIndex.cell} !important;
					background-color: ${summaryRowBackground};
					border-right-color: transparent !important;
					border-left-color: transparent !important;
				}

				.rdg-cell:not(:last-of-type) {
					grid-column: 1 / -1 !important;
				}

				.rdg-cell:nth-of-type(-n + ${MANAGEMENT_SUMMARY_LEFT_COLUMNS_COUNT}) {
					grid-column: 1 / ${MANAGEMENT_SUMMARY_LEFT_SPAN_END_COLUMN} !important;
				}

				.rdg-cell.${MANAGEMENT_SUMMARY_LEFT_CELL_CLASS} {
					z-index: ${summaryZIndex.left} !important;
				}

				.rdg-cell:nth-of-type(
						-n + ${MANAGEMENT_SUMMARY_LEFT_COLUMNS_COUNT}
					):not(.${MANAGEMENT_SUMMARY_LEFT_CELL_CLASS}) {
					pointer-events: none;
				}

				.rdg-cell:nth-of-type(${summaryValueColumnsStart}) {
					grid-column: ${summaryValueColumnsStart} !important;
					z-index: ${summaryZIndex.value} !important;
				}

				.rdg-cell:nth-of-type(${summaryValueColumnsStart + 1}) {
					grid-column: ${summaryValueColumnsStart + 1} !important;
					z-index: ${summaryZIndex.value} !important;
				}

				.rdg-cell:nth-of-type(${MANAGEMENT_SUMMARY_VALUE_COLUMNS_END}) {
					grid-column: ${MANAGEMENT_SUMMARY_VALUE_COLUMNS_END} !important;
					z-index: ${summaryZIndex.value} !important;
				}

				.rdg-cell:last-of-type {
					grid-column: ${summaryEditFromColumn} / ${summaryToColumn + 1} !important;
					z-index: ${summaryZIndex.edit} !important;
					background-color: ${summaryRowBackground} !important;
					border-left: 1px solid ${gridTheme.palette['graphics-divider-1']} !important;
					box-shadow: 100vw 0 0 0 ${summaryRowBackground} !important;
				}
			}
		`;
	};
