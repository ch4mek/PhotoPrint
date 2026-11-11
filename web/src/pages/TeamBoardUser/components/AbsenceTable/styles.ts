import { css, Theme } from '@emotion/react';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStyles,
} from '@/components/Grid';

import { ABSENCE_TABLE_COLUMNS_WIDTHS } from './constants';

export const overrideAbsenceSectionTableStyles = (theme: Theme) =>
	css`
		${overrideGridStyles(
			false,
			getGridColumnTemplateFromColumnWidths(ABSENCE_TABLE_COLUMNS_WIDTHS)
		)(theme).styles}

		.rdg-row {
			background-color: ${theme.palette['background-white']};
		}
	`;
