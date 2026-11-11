import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { overrideGridStylesWithEditColumn } from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { MIN_ABSENCE_HISTORY_HEIGHT } from '@/constants/absence';

export const overrideTransactionsHistoryGridStyles =
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

			.rdg-cell[role='columnheader']:last-of-type {
				border-right: none;
			}
		`;

export const GridWrapper = styled(DataGridHoverRowWrapper)({
	minHeight: MIN_ABSENCE_HISTORY_HEIGHT,
	maxHeight: MIN_ABSENCE_HISTORY_HEIGHT,
});
