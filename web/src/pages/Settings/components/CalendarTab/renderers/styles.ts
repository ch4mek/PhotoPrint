import { css } from '@emotion/react';
import styled from '@emotion/styled';

import { EDIT_CELL_RIGHT_PADDING } from '@/components/Grid/constants';

const SKELETON_HEIGHT = 40;

export const CellWrapper = styled('div')<{ fullHeight?: boolean }>(
	({ theme, fullHeight = true }) => ({
		width: '100%',
		height: fullHeight ? '100%' : undefined,
		display: 'flex',
		alignItems: 'center',
		justifyContent: 'center',
		padding: `0 ${theme.spaces.s}px`,
	})
);

export const TotalCellWrapper = styled(CellWrapper)(
	({ theme }) => css`
		padding: 0px ${EDIT_CELL_RIGHT_PADDING}px 0px ${theme.spaces.s}px;
	`
);

export const SkeletonCellWrapper = styled(CellWrapper)(
	() => css`
		& > span {
			width: 100%;
			height: ${SKELETON_HEIGHT}px;

			transform: none;
		}
	`
);
