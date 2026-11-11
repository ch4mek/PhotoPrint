import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';

import { CELL_WIDTH } from '../constants';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { scrollbarWidths } from '@/constants/commonStyles';

const TIMELINE_GRID_BORDER_MARGIN = 2;

export const TimelineGridWrapper = styled('div')<{ cellsAmount: number }>(
	({ theme, cellsAmount }) => ({
		position: 'relative',
		width: cellsAmount * CELL_WIDTH + TIMELINE_GRID_BORDER_MARGIN,

		border: `1px solid ${theme.palette['graphics-input-stroke']}`,
		borderTopRightRadius: BorderRadiuses.xs2,
		borderBottomRightRadius: BorderRadiuses.xs2,
		...getScrollbarStyles(theme, scrollbarWidths.m, 'horizontal'),
	})
);

export const TimelineGridCellsWrapper = styled('div')({
	position: 'relative',
});
