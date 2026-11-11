import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { GridColumnWidthsConfiguration } from '@/types/grid';

export const RATE_HISTORY_RATE_COLUMN_HEADER_CLASS =
	'rate-history-rate-column-header';

export const RATE_HISTORY_GRID_EDIT_COLUMN_WIDTH = 44;

export const RATE_HISTORY_MAX_VISIBLE_ROWS_COUNT = 5;

export const RATE_HISTORY_GRID_HEIGHT =
	COMMON_GRID_PROPS.headerRowHeight +
	RATE_HISTORY_MAX_VISIBLE_ROWS_COUNT * COMMON_GRID_PROPS.rowHeight;

export const rateHistoryGridColumnsWidths = {
	date: 100,
	rate: 100,
} satisfies GridColumnWidthsConfiguration;

export const RATE_HISTORY_GRID_EDIT_COLUMN_INDEX = 4;

export const rateHistoryGridColumns = {
	date: {
		key: 'date',
		name: 'Date',
		sortable: false,
	},
	rate: {
		key: 'rate',
		name: 'Rate/h',
		sortable: false,
	},
};

export const RATE_HISTORY_GRID_HEADER_TITLE = 'Rate details';
