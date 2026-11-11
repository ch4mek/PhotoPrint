import { css, Theme } from '@emotion/react';

import { overrideGridStylesWithEditColumn } from '@/components/Grid';
import { EDIT_COLUMN_CELL_CLASS } from '@/components/Grid/constants';
import { alternateTabTableRowsColor } from '@/pages/TeamBoardUser/constants';

const tabModeStyles = (theme: Theme) => css`
	flex-grow: 1;
	min-height: 0;
	${alternateTabTableRowsColor(theme).styles};
`;

export const overridePerformanceReviewGridStyles =
	(
		isPaginationShown: boolean,
		columnIndex?: number,
		templateColumns?: string
	) =>
	(theme: Theme) =>
		css`
			${overrideGridStylesWithEditColumn(
				isPaginationShown,
				columnIndex,
				templateColumns
			)(theme).styles}

			${tabModeStyles(theme).styles}

			& .rdg-cell.${EDIT_COLUMN_CELL_CLASS} {
				padding-top: ${theme.spaces.xs}px;
				display: flex;
				justify-content: center;
			}

			.rdg-cell {
				display: flex;
				align-items: flex-start;
				padding-top: ${theme.spaces.xs}px;
				line-height: ${theme.typography.body['m-400'].lineHeight};
			}
		`;
