import { useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	RATE_HISTORY_RATE_COLUMN_HEADER_CLASS,
	rateHistoryGridColumns,
	rateHistoryGridColumnsWidths,
} from '../constants';
import { RateHistoryGridRow } from '../types';

import {
	editCellRenderer,
	getColumnFactory,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
} from '@/components/Grid';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { RateCellRenderer } from '@/pages/ProfitAndLoss/components/RatesTab/components/renderers/RateCellRenderer';

export const useRateHistoryColumns = (): Column<RateHistoryGridRow>[] =>
	useMemo(
		() => [
			getColumnWithTooltipFactory({
				...rateHistoryGridColumns.date,
				width: getColumnWidth(rateHistoryGridColumnsWidths.date).width,
			}),
			getColumnFactory({
				...rateHistoryGridColumns.rate,
				width: getColumnWidth(rateHistoryGridColumnsWidths.rate).width,
				renderCell: RateCellRenderer,
				headerCellClass: RATE_HISTORY_RATE_COLUMN_HEADER_CLASS,
			}),
			getEditColumnFactory({
				key: '',
				name: '',
				displayFilterButton: false,
				disabled: false,
				renderCell: editCellRenderer,
				frozen: true,
				sortable: false,
			}),
		],
		[]
	);
