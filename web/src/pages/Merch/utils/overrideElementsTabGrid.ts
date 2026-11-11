import { css, Theme } from '@emotion/react';

import { getColumnIndexForStyle } from '../components/ElementsTab/utils/getColumnIndexForStyle';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { BASE_LINE_HEIGHT } from '@/constants/commonStyles';
import { GridColumnWidthsConfiguration } from '@/types/grid';

export const overrideMerchGridStyles =
	(
		isPaginationShown: boolean,
		gridColumnWidths: GridColumnWidthsConfiguration
	) =>
	(theme: Theme) => {
		const templateColumns = getGridColumnTemplateFromColumnWidths(
			gridColumnWidths,
			true
		);

		return css`
			.rdg-cell[role='gridcell'] {
				display: flex;
				align-items: flex-start;
				padding: 0;
				border-inline-end: none;
				line-height: ${BASE_LINE_HEIGHT};
			}
			.edit-column-cell {
				padding-top: ${theme.spaces.m}px !important;
			}
			${overrideGridStylesWithEditColumn(
				isPaginationShown,
				getColumnIndexForStyle(),
				templateColumns
			)(theme)}
		`;
	};
