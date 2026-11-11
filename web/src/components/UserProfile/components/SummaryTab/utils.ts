import { css, Theme } from '@emotion/react';

import { overrideGridStylesWithEditColumn } from '@/components/Grid';

export const overrideSectionGridStyles =
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
			)(theme)}

			.rdg-row {
				background-color: ${theme.palette['background-white']};
			}

			border-color: ${theme.palette['graphics-input-stroke']};
		`;
