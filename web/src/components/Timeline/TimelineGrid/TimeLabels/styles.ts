import styled from '@emotion/styled';

import { CELL_WIDTH, TIME_LABELS_HEIGHT } from '../../constants';

export const TimeLabelsWrapper = styled('div')<{
	cellsAmount: number;
}>(({ theme, cellsAmount }) => ({
	height: TIME_LABELS_HEIGHT,

	display: 'grid',
	alignItems: 'center',
	gridTemplateColumns: `repeat(${cellsAmount},${CELL_WIDTH}px)`,
	width: cellsAmount * CELL_WIDTH,
	color: theme.palette['text-black'],

	'> *:not(:last-child)': {
		borderRight: `1px solid ${theme.palette['graphics-divider-1']}`,
	},

	span: {
		display: 'flex',
		alignItems: 'center',
		height: '100%',
		paddingLeft: theme.spaces.s,
	},
}));
