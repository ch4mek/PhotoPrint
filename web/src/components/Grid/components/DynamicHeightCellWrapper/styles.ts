import { css } from '@emotion/react';
import styled from '@emotion/styled';

const maxHeightStyles = (maxHeight: number) => css`
	display: -webkit-box;
	line-clamp: ${maxHeight};
	-webkit-line-clamp: ${maxHeight};
	-webkit-box-orient: vertical;
	text-overflow: ellipsis !important;
	overflow: hidden !important;
	white-space: nowrap !important;
`;

export const DynamicCellWrapper = styled('div')<{ maxTextRowCount?: number }>(
	({ maxTextRowCount }) => css`
		width: 100%;
		height: fit-content;
		${maxTextRowCount && maxHeightStyles(maxTextRowCount)};
	`
);
