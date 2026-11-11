import { css, Theme } from '@emotion/react';

import { overrideGridStylesWithEditColumn } from '@/components/Grid';
import { EDIT_COLUMN_CELL_CLASS } from '@/components/Grid/constants';

export const overridePayslipsGridStyles =
	(isPaginationShown: boolean, columnIndex: number, templateColumns?: string) =>
	(theme: Theme) =>
		css`
			${overrideGridStylesWithEditColumn(
				isPaginationShown,
				columnIndex,
				templateColumns
			)(theme)}

			${css`
				.rdg-cell.${EDIT_COLUMN_CELL_CLASS} {
					display: none;
				}
			`}
		`;
