import styled from '@emotion/styled';
import { Skeleton } from '@mui/material';

import { CELL_HEIGHT, CELL_WIDTH } from '../constants';

import { checkIfForwardProps } from '@/helpers/checkIfForwardProps';

const TIMELINE_SKELETON_PROPS_TO_CHECK = ['cellsAmount'];

export const TimelineRowWrapper = styled('div')<{
	cellsAmount: number;
	timelineSectionsHeight: number;
}>(({ theme, cellsAmount, timelineSectionsHeight }) => ({
	position: 'relative',
	display: 'grid',
	alignItems: 'center',

	gridTemplateColumns: `repeat(${cellsAmount},${CELL_WIDTH}px)`,
	borderTop: `1px solid ${theme.palette['graphics-divider-1']}`,
	width: cellsAmount * CELL_WIDTH,
	height: timelineSectionsHeight,
	overflow: 'hidden',
}));

export const TimelineRowSkeleton = styled(Skeleton, {
	shouldForwardProp: checkIfForwardProps(TIMELINE_SKELETON_PROPS_TO_CHECK),
})<{ cellsAmount: number }>(({ theme, cellsAmount }) => ({
	marginLeft: theme.spaces.s,
	height: CELL_HEIGHT,
	width: cellsAmount * CELL_WIDTH - theme.spaces.m,
}));
