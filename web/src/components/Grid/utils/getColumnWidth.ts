import { ColumnWidthConfig } from '../types';

import { GridColumnWidth } from '@/types/grid';

const COLUMN_CONFIG_FOR_NULL = {
	width: 0,
	fixed: true,
	maxWidth: 0,
};

export const getColumnWidth = (
	columnWidth: GridColumnWidth
): ColumnWidthConfig => {
	if (!columnWidth) {
		return COLUMN_CONFIG_FOR_NULL;
	}

	if (typeof columnWidth === 'number' || typeof columnWidth === 'string') {
		return {
			width: columnWidth,
			fixed: false,
		};
	}

	if ('width' in columnWidth) {
		return columnWidth.width ? columnWidth : COLUMN_CONFIG_FOR_NULL;
	}

	return COLUMN_CONFIG_FOR_NULL;
};
