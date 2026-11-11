import { css, Theme } from '@emotion/react';

import {
	RATE_HISTORY_GRID_EDIT_COLUMN_INDEX,
	RATE_HISTORY_GRID_EDIT_COLUMN_WIDTH,
	RATE_HISTORY_RATE_COLUMN_HEADER_CLASS,
	rateHistoryGridColumnsWidths,
} from '../constants';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideSectionTableStyles,
} from '@/components/Grid';

export const getOverrideRateHistoryGridStyles = () => (gridTheme: Theme) => {
	const gridColumnTemplate = getGridColumnTemplateFromColumnWidths(
		rateHistoryGridColumnsWidths,
		true,
		0,
		true
	);

	const sectionStyles = overrideSectionTableStyles(
		false,
		RATE_HISTORY_GRID_EDIT_COLUMN_INDEX,
		gridColumnTemplate,
		RATE_HISTORY_GRID_EDIT_COLUMN_WIDTH
	);

	const rateColumnHeaderCellSelector = `.rdg-cell.${RATE_HISTORY_RATE_COLUMN_HEADER_CLASS}`;
	const rateColumnHeaderCellContentSelector = `${rateColumnHeaderCellSelector} > div`;

	return css`
		${sectionStyles(gridTheme)}

		height: 100%;
		block-size: 100%;

		${rateColumnHeaderCellSelector} {
			text-align: right;
		}

		${rateColumnHeaderCellContentSelector},
		${rateColumnHeaderCellContentSelector} > div {
			justify-content: flex-end;
		}
	`;
};
