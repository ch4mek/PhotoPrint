import { css, Theme } from '@emotion/react';

import { COLUMNS_WIDTH, COLUMNS_AMOUNT } from '../constants';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
	overrideGridStyles,
} from '@/components/Grid';
import { GRID_FROZEN_EDIT_COLUMN_DIFF } from '@/pages/constants';

const EQUIPMENT_EDIT_COLUMN_WIDTH = 44;
const EQUIPMENT_SCROLLBAR_SIZE = 3;

const ovverideEquipmentGridScrollbar = (isHistoryTabActive: boolean) => ({
	'&::-webkit-scrollbar': {
		width: EQUIPMENT_SCROLLBAR_SIZE,
		height: EQUIPMENT_SCROLLBAR_SIZE,
		'&-track': {
			marginRight: isHistoryTabActive ? EQUIPMENT_EDIT_COLUMN_WIDTH : 0,
		},
	},
});

export const overrideEquipmentGridStyles =
	(gridHasPagination: boolean, isHistoryTabActive: boolean) =>
	(theme: Theme) => {
		const gridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			COLUMNS_WIDTH,
			isHistoryTabActive
		);

		if (isHistoryTabActive) {
			return css`
				${overrideGridStylesWithEditColumn(
					gridHasPagination,
					COLUMNS_AMOUNT + GRID_FROZEN_EDIT_COLUMN_DIFF,
					gridColumnTemplate,
					EQUIPMENT_EDIT_COLUMN_WIDTH
				)(theme)}
				${ovverideEquipmentGridScrollbar(isHistoryTabActive)}
			`;
		}

		return css`
			${overrideGridStyles(gridHasPagination, gridColumnTemplate)(theme)}
			${ovverideEquipmentGridScrollbar(isHistoryTabActive)}
		`;
	};
