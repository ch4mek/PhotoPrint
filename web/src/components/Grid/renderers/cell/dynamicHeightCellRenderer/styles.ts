import { css, SerializedStyles, Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { EDIT_COLUMN_CELL_CLASS } from '@/components/Grid/constants';

const maxHeightStyles = (maxHeight: number) => css`
	display: -webkit-box;
	line-clamp: ${maxHeight};
	-webkit-line-clamp: ${maxHeight};
	-webkit-box-orient: vertical;
	text-overflow: ellipsis !important;
	overflow: hidden !important;
	white-space: pre-wrap !important;
`;

export const DynamicCellWrapper = styled('div')<{ maxTextRowCount?: number }>(
	({ maxTextRowCount }) => css`
		width: 100%;
		height: fit-content;
		white-space: pre-wrap !important;
		${maxTextRowCount && maxHeightStyles(maxTextRowCount)};
	`
);

export const overrideGridStylesWithDynamicCell =
	(additionalStyles?: (theme: Theme) => SerializedStyles) => (theme: Theme) =>
		css`
			${additionalStyles?.(theme).styles}

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
