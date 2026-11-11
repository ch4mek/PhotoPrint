import styled from '@emotion/styled';

import { DEFAULT_DISABLED_GRID_CELL_OPACITY } from '@/constants/opacities';

export const ColoredMerchCell = styled('div')<{ color?: string }>(
	({ color }) => color && { backgroundColor: color }
);

export const DisabledColoredMerchCell = styled(ColoredMerchCell)<{
	disabled?: boolean;
}>(({ theme, disabled }) => ({
	...(disabled && {
		opacity: DEFAULT_DISABLED_GRID_CELL_OPACITY,
		backgroundColor: theme.palette['background-white'],
	}),
}));

export const ColoredEditCellWrapper = styled(ColoredMerchCell)<{
	disabled?: boolean;
}>(({ theme, disabled, color }) => ({
	position: 'absolute',
	alignSelf: 'flex-end',
	width: '100%',
	height: '100%',
	display: 'flex',
	padding: `${theme.spaces.m - 1}px ${theme.spaces.s}px`,
	backgroundColor: disabled ? undefined : color,

	'& svg': {
		cursor: 'pointer',
	},

	...(disabled && {
		'&::before': {
			content: '""',
			position: 'absolute',
			inset: 0,
			backgroundColor: theme.palette['background-white'],
			opacity: DEFAULT_DISABLED_GRID_CELL_OPACITY,
			pointerEvents: 'none',
		},
		'& > *': {
			position: 'relative',
			zIndex: 1,
		},
	}),
}));
