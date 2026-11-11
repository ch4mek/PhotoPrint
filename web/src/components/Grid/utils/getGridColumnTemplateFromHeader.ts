import { css, Theme } from '@emotion/react';
import { DEFAULT_GRID_HEADER_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';

import {
	createActiveBorderColor,
	createBorderColor,
	getTotalColumnBackgroundColor,
} from '@/helpers/getColor';
import {
	MANAGER_TOTAL_COLUMN_HEADER_CLASS_NAME,
	PRIMARY_ROW_TOTAL_CELL_CLASS_NAME,
} from '@/pages/TimeSheet/components/TimeSheeetManager/constants';
import {
	MONTH_MINIMAL_TRACK_CELL_WIDTH,
	TIMESHEET_TOTAL_COLUMN_WIDTH,
	TIMESHEET_USER_NAME_COLUMN_WIDTH,
	TIMESHEET_USER_NAME_COLUMN_WIDTH_MOBILE,
	WEEK_MINIMAL_TRACK_CELL_WIDTH,
	TOTAL_COLUMN_HEADER_CLASS_NAME,
	TOTAL_COLUMN_CELL_CLASS_NAME,
} from '@/pages/TimeSheet/constants';
import { isMonthPeriod } from '@/pages/TimeSheet/utils';
import { GridColumnWidth, GridColumnWidthsConfiguration } from '@/types/grid';

import { getColumnWidth } from './getColumnWidth';

export const calculateMinWidth = (
	columnWidthValues: Array<GridColumnWidth>
) => {
	const columnWidths = columnWidthValues.filter(
		(item): item is Exclude<GridColumnWidth, string> => typeof item !== 'string'
	);

	return Math.min(
		...columnWidths
			.map((value) => getColumnWidth(value))
			.filter((value) => value?.fixed !== true && Boolean(value.width))
			.map((value) => value.width as number)
	);
};

export const getGridColumnTemplateFromColumnWidths = (
	columnWidths: GridColumnWidthsConfiguration,
	hideFirstColumn = false,
	minWidth = 0,
	makeColumnsEqual = false
) => {
	const columnWidthValues = Object.values(columnWidths);
	const calculatedMinWidth =
		minWidth || calculateMinWidth(columnWidthValues) || 1;

	return `grid-template-columns: ${
		hideFirstColumn ? '0px' : ''
	} ${columnWidthValues
		.map((columnWidth) => {
			const extractedWidth = getColumnWidth(columnWidth);

			if (!extractedWidth.width) {
				return '';
			}

			const { width, fixed, maxWidth } = extractedWidth;

			if (fixed) {
				return `${width}px`;
			}

			if (typeof width === 'string') {
				return width;
			}

			if (typeof maxWidth === 'number' && maxWidth > width) {
				return `minmax(${width}px, ${maxWidth}px)`;
			}

			return `minmax(${width}px, ${
				makeColumnsEqual ? 1 : (width / calculatedMinWidth).toFixed(2)
			}fr)`;
		})
		.join(' ')} !important;`;
};

export const commonTimesheetGridStyles =
	(periodLength: number, isMobileView: boolean, hideFirstColumn = false) =>
	(theme: Theme) =>
		css`
			.rdg-cell {
				span {
					font-size: ${theme.typography.body['m-400'].fontSize};
					line-height: ${theme.typography.body['m-400'].lineHeight};
				}

				.error-text span {
					font-size: ${theme.typography.body['s-400'].fontSize};
				}

				&[role='columnheader'] {
					span {
						font-size: ${theme.typography.body['s-600'].fontSize};
						font-weight: ${theme.typography.body['s-600'].fontWeight};
						color: ${theme.palette['text-gray']};
					}

					height: ${DEFAULT_GRID_HEADER_ROW_HEIGHT}px;
					padding: 0 ${theme.spaces.s}px;

					&.${MANAGER_TOTAL_COLUMN_HEADER_CLASS_NAME},
						&.${TOTAL_COLUMN_HEADER_CLASS_NAME} {
						border-right: none !important;
						border-left: 1px solid ${createBorderColor(theme)};
					}
				}

				&.${TOTAL_COLUMN_CELL_CLASS_NAME},
					&.${PRIMARY_ROW_TOTAL_CELL_CLASS_NAME} {
					border-left: 1px solid ${createActiveBorderColor(theme)};
					background-color: ${getTotalColumnBackgroundColor(theme)};
					border-top-color: ${getTotalColumnBackgroundColor(theme)};
					border-bottom-color: ${getTotalColumnBackgroundColor(theme)};
				}
			}

			display: grid;
			grid-template-columns:
				${hideFirstColumn ? '0px' : ''} ${isMobileView
					? TIMESHEET_USER_NAME_COLUMN_WIDTH_MOBILE
					: TIMESHEET_USER_NAME_COLUMN_WIDTH}px repeat(
					${periodLength},
					minmax(
						${isMonthPeriod(periodLength)
							? MONTH_MINIMAL_TRACK_CELL_WIDTH
							: WEEK_MINIMAL_TRACK_CELL_WIDTH}px,
						1fr
					)
				)
				${TIMESHEET_TOTAL_COLUMN_WIDTH}px !important;
		`;
