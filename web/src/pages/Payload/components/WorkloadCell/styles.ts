import styled from '@emotion/styled';
import { IMaskInput } from 'react-imask';

import {
	PAYLOAD_BORDER_WIDTH,
	WORKLOAD_SUBROW_CLASS_NAME,
	WORKLOAD_SUBROW_LAST_CELL_CLASS_NAME,
} from '../../contants';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import {
	DEFAULT_DISABLED_GRID_CELL_OPACITY,
	DEFAULT_OPACITY,
	HALF_OPACITY,
} from '@/constants/opacities';
import { ThemeMode } from '@/theme';

import { PayloadZIndexes } from './constants';
import {
	LowerDateCellAttrs,
	UpperDateCellAttrs,
	WorkloadCellWrapperAttrs,
} from './types';

const ICONS_SIZE = 32;
const ICONS_CONTAINER_WIDTH = '101%';

const SUBROW_APPEAR_ANIMATION_NAME = 'appear';
const SUBROW_APPEAR_ANIMATION_DELAY = '400ms';
const SUBROW_APPEAR_ANIMATION_DURATION = '200ms';
const SUBROW_APPEAR_TRANSLATE_OFFSET = '-6px';

export const UpperDateCell = styled('span')<UpperDateCellAttrs>(
	({
		theme,
		backgroundColor,
		isSubRows,
		isDisabled,
		minHeight,
		isClickable,
	}) => ({
		backgroundColor,
		display: 'flex',
		alignItems: 'center',
		justifyContent: 'flex-end',
		borderBottom: `1px solid ${theme.palette['graphics-divider-2']}`,
		cursor: isSubRows || isDisabled || !isClickable ? 'default' : 'pointer',
		pointerEvents: isDisabled ? 'none' : 'auto',
		opacity: isDisabled ? HALF_OPACITY : DEFAULT_OPACITY,
		minHeight,
	})
);
export const LowerDateCell = styled('span')<LowerDateCellAttrs>(
	({ height, disabled = false }) => ({
		display: 'flex',
		alignItems: 'center',
		justifyContent: 'flex-end',
		zIndex: PayloadZIndexes.zero,
		height,

		...(disabled && {
			userSelect: 'none',
			pointerEvents: 'none',
			opacity: DEFAULT_DISABLED_GRID_CELL_OPACITY,
		}),
	})
);

export const WorkloadCellWrapper = styled('div')<WorkloadCellWrapperAttrs>(
	({ maxHeight, theme }) => ({
		display: 'flex',
		flexDirection: 'column',
		height: '100%',
		position: 'relative',
		maxHeight,
		overflow: 'visible',
		boxShadow: `0 1px 0 0 ${theme.palette['graphics-divider-2']}`,

		[`&.${WORKLOAD_SUBROW_CLASS_NAME}`]: {
			opacity: 0,
			animation: `${SUBROW_APPEAR_ANIMATION_NAME} ${SUBROW_APPEAR_ANIMATION_DURATION} ease-out ${SUBROW_APPEAR_ANIMATION_DELAY} forwards`,
			[`@keyframes ${SUBROW_APPEAR_ANIMATION_NAME}`]: {
				from: {
					opacity: 0,
					transform: `translateY(${SUBROW_APPEAR_TRANSLATE_OFFSET})`,
				},
				to: { opacity: 1 },
			},
		},

		[`&:is(.${WORKLOAD_SUBROW_LAST_CELL_CLASS_NAME})`]: {
			boxShadow: `${PAYLOAD_BORDER_WIDTH}px 0 0 ${theme.palette['graphics-divider-2']}`,
			zIndex: PayloadZIndexes.ten,
		},
	})
);
export const PlannedTimeInput = styled(IMaskInput)(({ theme }) => ({
	backgroundColor: 'transparent',
	width: '102%',
	height: '100%',
	color:
		theme.mode === ThemeMode.LIGHT
			? theme.palette['text-black']
			: theme.palette['text-white'],
	textAlign: 'right',
	paddingRight: theme.spaces.s,
	marginRight: 1,
	'&:focus': {
		outline: 'none',
		boxShadow: `inset 0 0 0 2px ${theme.palette['graphics-accent']}`,
		zIndex: PayloadZIndexes.xxl,
	},
	borderRadius: BorderRadiuses.xs,
	...theme.typography.body['m-700'],
}));

export const PlannedTimeInputWrapper = styled.div`
	display: flex;
	flex-direction: column;
	height: 100%;
	justify-content: center;
	align-items: center;
	position: relative;
	background: transparent;
	width: 100%;
`;

export const IconsContainer = styled('div')<{
	isLastRow: boolean;
	minHeight: number;
	shouldPlaceButtonsAbove: boolean;
}>(({ theme, isLastRow, minHeight, shouldPlaceButtonsAbove }) => ({
	position: 'absolute',
	display: 'flex',
	justifyContent: 'flex-end',
	gap: theme.spaces.xs,
	width: ICONS_CONTAINER_WIDTH,
	bottom:
		isLastRow && shouldPlaceButtonsAbove
			? minHeight + theme.spaces.xs
			: -theme.spaces.xl - theme.spaces.xs,

	'& > button': {
		zIndex: PayloadZIndexes.six,
		width: ICONS_SIZE,
		height: ICONS_SIZE,
	},
}));
