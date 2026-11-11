import { css } from '@emotion/react';
import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { DEFAULT_OPACITY, HALF_OPACITY } from '@/constants/opacities';
import { timesheetZIndexes } from '@/pages/TimeSheet/constants';

import { CellInputWrapperProps } from './types';

const INPUT_WRAPPER_DEFAULT_WIDTH = 56;
const INPUT_HEIGHT = 40;
const DOUBLE_CELL_INPUT_HEIGHT = 30;

const topCellBorderStyles = css`
	border-bottom-left-radius: 0 !important;
	border-bottom-right-radius: 0 !important;
`;

const bottomCellBorderStyles = css`
	margin-top: -2px;
	border-top-left-radius: 0 !important;
	border-top-right-radius: 0 !important;
`;

export const InputWrapper = styled('div')<CellInputWrapperProps>(
	({ theme, disabled, cellPosition }) => ({
		width: INPUT_WRAPPER_DEFAULT_WIDTH,
		display: 'flex',
		alignItems: 'center',
		opacity: disabled ? HALF_OPACITY : DEFAULT_OPACITY,
		height: DOUBLE_CELL_INPUT_HEIGHT,

		input: {
			height: cellPosition ? DOUBLE_CELL_INPUT_HEIGHT : INPUT_HEIGHT,
			borderRadius: BorderRadiuses.m,
			border: `1px solid ${theme.palette['graphics-input-stroke']}`,
			color: theme.palette['text-black'],
			textAlign: 'center',
			width: INPUT_WRAPPER_DEFAULT_WIDTH,
			padding: 0,
			...theme.typography.body['m-400'],
			background: theme.palette['background-white'],
			'&:hover': {
				textDecoration: 'none',
				border: `1px solid ${
					disabled
						? theme.palette['graphics-input-stroke']
						: theme.palette['graphics-primary-2']
				}`,
				zIndex: timesheetZIndexes.hoverCell,
			},
			'&:focus': {
				outline: 'none',
				border: `1px solid ${theme.palette['graphics-accent']}`,
				zIndex: timesheetZIndexes.activeCell,
			},
		},
	}),
	({ cellPosition }) =>
		cellPosition &&
		css`
			input {
				${cellPosition === 'bottom'
					? bottomCellBorderStyles
					: topCellBorderStyles};
			}
		`
);
