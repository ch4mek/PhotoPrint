import {
	FIXED_COLUMNS_WIDTH,
	MINIMAL_MONTH_COLUMN_WIDTH,
} from '../../constants';

import { months } from '@/constants/date';

const periodColumnsLength = months.length;

const getMinimalRequiredGridLength = (
	minimalColumnWidth: number,
	fixedColumnsWidth: number
) => fixedColumnsWidth + periodColumnsLength * minimalColumnWidth;

const getColumnWidthFromGridOffsetWidth = (
	gridOffsetWidth: number,
	fixedColumnsWidth: number
) => (gridOffsetWidth - fixedColumnsWidth) / periodColumnsLength;

export const getMonthColumnWidth = (gridOffsetWidth: number) =>
	getMinimalRequiredGridLength(
		MINIMAL_MONTH_COLUMN_WIDTH,
		FIXED_COLUMNS_WIDTH
	) > gridOffsetWidth
		? MINIMAL_MONTH_COLUMN_WIDTH
		: getColumnWidthFromGridOffsetWidth(gridOffsetWidth, FIXED_COLUMNS_WIDTH);

export const isExpensesGridCompactView = (monthColumnWidth: number) =>
	monthColumnWidth === MINIMAL_MONTH_COLUMN_WIDTH;
