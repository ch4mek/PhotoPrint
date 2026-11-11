import { css, Theme } from '@emotion/react';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { BASE_LINE_HEIGHT } from '@/constants/commonStyles';
import { GridColumnWidthsConfiguration } from '@/types/grid';

import { getColumnIndexForStyle } from './getColumnIndexForStyle';

export const overrideMicrofrontendsGridStyles =
	(
		isPaginationShown: boolean,
		gridColumnWidths: GridColumnWidthsConfiguration
	) =>
	(theme: Theme) => {
		const templateColumns = getGridColumnTemplateFromColumnWidths(
			gridColumnWidths,
			false
		);

		return css`
			.rdg-cell[role='gridcell'] {
				display: flex;
				align-items: start;
				padding: ${theme.spaces.m}px;
				border-inline-end: none;
				line-height: ${BASE_LINE_HEIGHT};
			}

			${overrideGridStylesWithEditColumn(
				isPaginationShown,
				getColumnIndexForStyle(),
				templateColumns
			)(theme)}
		`;
	};
