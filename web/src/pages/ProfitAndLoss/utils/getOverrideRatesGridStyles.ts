import { css, Theme } from '@emotion/react';

import {
	RATES_CHILD_GROUP_CELL_CLASS,
	RATES_RATE_COLUMN_HEADER_CLASS,
	RATES_CHILD_ROW_FONT_SIZE,
	RATES_CHILD_ROW_HEIGHT,
	ratesRowClasses,
	RIGHT_ALIGN_CELL_CLASS,
} from '../constants';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { zIndexes } from '@/constants/zIndexes';
import {
	getEvenRowBackgroundColor,
	getHoverBackgroundColor,
	getOddRowBackgroundColor,
} from '@/helpers/getColor';
import { GridColumnWidthsConfiguration } from '@/types/grid';

const getRatesGridRowSelector = (rowClass: string) => `.rdg-row.${rowClass}`;

const getRatesGridCellNotHoverSelector = (rowClass: string) =>
	`.rdg-row.${rowClass}:not(:hover) > .rdg-cell:not([role='columnheader'])`;

const getRatesGridCellHoverSelector = (rowClass: string) =>
	`.rdg-row.${rowClass}:hover > .rdg-cell:not([role='columnheader'])`;

export const getOverrideRatesGridStyles =
	(columnsWidths: GridColumnWidthsConfiguration, rowsLength: number) =>
	(gridTheme: Theme) => {
		const getGridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			columnsWidths,
			true
		);

		const rowDividerColor = gridTheme.palette['graphics-divider-2'];

		const {
			rowEven,
			rowOdd,
			parentRow,
			parentRowExpanded,
			childRow,
			childRowLast,
		} = ratesRowClasses;

		const rowEvenNotHoverCellsSelector =
			getRatesGridCellNotHoverSelector(rowEven);
		const rowOddNotHoverCellsSelector =
			getRatesGridCellNotHoverSelector(rowOdd);
		const rowEvenHoverCellsSelector = getRatesGridCellHoverSelector(rowEven);
		const rowOddHoverCellsSelector = getRatesGridCellHoverSelector(rowOdd);
		const parentRowExpandedChildGroupCellSelector = `.${parentRowExpanded} .${RATES_CHILD_GROUP_CELL_CLASS}::after`;
		const childRowChildGroupCellSelector = `.${childRow}:not(.${childRowLast}) .${RATES_CHILD_GROUP_CELL_CLASS}::after`;
		const childRowCellsSelector = `${getRatesGridRowSelector(
			childRow
		)} > .rdg-cell`;
		const childRowCellContentSelector = `${childRowCellsSelector} > div`;
		const rateColumnHeaderCellSelector = `.rdg-cell.${RATES_RATE_COLUMN_HEADER_CLASS}`;
		const rightAlignCellSelector = `.rdg-cell.${RIGHT_ALIGN_CELL_CLASS}`;
		const rateColumnHeaderCellContentSelector = `${rateColumnHeaderCellSelector} > div`;
		const parentRowRateColumnCellContentSelector = `${getRatesGridRowSelector(
			parentRow
		)} > ${rateColumnHeaderCellSelector} > div, ${getRatesGridRowSelector(
			parentRowExpanded
		)} > ${rateColumnHeaderCellSelector} > div`;
		const editColumnIndex = Object.keys(columnsWidths).length + 2;

		const extendedStyles = overrideGridStylesWithEditColumn(
			!!rowsLength,
			editColumnIndex,
			getGridColumnTemplate
		);

		return css`
			${extendedStyles(gridTheme)}

			.rdg-row.rdg-row-odd,
			.rdg-row.rdg-row-even {
				background-color: unset;
			}

			.rdg-summary-row {
				.rdg-cell {
					position: sticky !important;
					z-index: ${zIndexes.logOutPopUp} !important;
					background-color: ${gridTheme.palette['background-base-2']};
				}
			}

			${getRatesGridRowSelector(rowEven)} {
				background-color: ${getEvenRowBackgroundColor(gridTheme)};
			}

			${getRatesGridRowSelector(rowOdd)} {
				background-color: ${getOddRowBackgroundColor(gridTheme)};
			}

			${rowEvenNotHoverCellsSelector},
			${rowOddNotHoverCellsSelector} {
				background-color: inherit;
			}

			${rowEvenHoverCellsSelector},
			${rowOddHoverCellsSelector} {
				background-color: ${getHoverBackgroundColor(gridTheme)};
			}

			@media (hover: hover) {
				.rdg-row:hover .rdg-cell:not([role='columnheader'])::after,
				.rdg-row:has(+ .rdg-row:hover)
					.rdg-cell:not([role='columnheader'])::after {
					background: ${rowDividerColor};
				}
			}

			@media (hover: none) {
				.rdg-row:active .rdg-cell:not([role='columnheader'])::after,
				.rdg-row:has(+ .rdg-row:active)
					.rdg-cell:not([role='columnheader'])::after {
					background: ${rowDividerColor};
				}
			}

			${parentRowExpandedChildGroupCellSelector} {
				height: 0px;
			}

			${childRowChildGroupCellSelector} {
				height: 0px;
			}

			${childRowCellsSelector} {
				font-size: ${RATES_CHILD_ROW_FONT_SIZE};
				align-items: center;
			}

			${childRowCellContentSelector} {
				height: 100%;
				width: 100%;
				min-width: 0;
				overflow: hidden;
				text-overflow: ellipsis;
				white-space: nowrap;
				line-height: ${RATES_CHILD_ROW_HEIGHT}px;
			}

			${rateColumnHeaderCellSelector}, ${rightAlignCellSelector} {
				text-align: right;
			}

			${rateColumnHeaderCellContentSelector},
			${rateColumnHeaderCellContentSelector} > div {
				justify-content: flex-end;
			}

			${parentRowRateColumnCellContentSelector} {
				display: flex;
				justify-content: flex-end;
				align-items: center;
				width: 100%;
				height: 100%;
				min-width: 0;
			}
		`;
	};
