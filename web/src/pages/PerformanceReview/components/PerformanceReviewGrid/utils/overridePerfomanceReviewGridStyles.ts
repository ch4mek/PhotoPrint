import { css, Theme } from '@emotion/react';

import { getPerformanceReviewColumnsAmount } from '../constants';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { Roles } from '@/constants/roles';
import { GRID_FROZEN_EDIT_COLUMN_DIFF } from '@/pages/constants';
import { GridColumnWidthsConfiguration } from '@/types/grid';

const cellsCustomStyles = css`
	.rdg-cell {
		overflow: initial;
	}
`;

export const overridePerformanceReviewGridStyles =
	(
		isPaginationShown: boolean,
		gridColumnWidths: GridColumnWidthsConfiguration,
		role: Roles
	) =>
	(theme: Theme) => {
		const templateColumns = getGridColumnTemplateFromColumnWidths(
			gridColumnWidths,
			true
		);

		return css`
			${cellsCustomStyles}
			${overrideGridStylesWithEditColumn(
				isPaginationShown,
				getPerformanceReviewColumnsAmount(role) + GRID_FROZEN_EDIT_COLUMN_DIFF,
				templateColumns
			)(theme)}
		`;
	};
