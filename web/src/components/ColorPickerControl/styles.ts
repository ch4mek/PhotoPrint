import { Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import {
	DEFAULT_DISABLED_INPUT_COMPONENT_OPACITY,
	DEFAULT_OPACITY,
} from '@/constants/opacities';

import {
	ColorPickerButtonProps,
	ColorPickerWrapperProps,
	RandomColorsWrapperProps,
} from './types';

const COLOR_PICKER_MIN_WIDTH = 125;
const COLOR_PICKER_PADDING = 10;
const RANDOM_COLOR_GAP = 10;

export const ColorPickerButton = styled('button')<ColorPickerButtonProps>(
	({ theme, disabled, isActive }) => ({
		display: 'flex',
		alignItems: 'center',
		gap: theme.spaces.s,

		width: COLOR_PICKER_MIN_WIDTH,
		height: theme.spaces.xl2,
		padding: `${COLOR_PICKER_PADDING}px ${theme.spaces.m}px`,

		border: `1px solid ${theme.palette['graphics-input-stroke']}`,
		borderRadius: BorderRadiuses.m,

		color: theme.palette['text-black'],
		backgroundColor: 'transparent',
		cursor: disabled ? 'auto' : 'pointer',

		'@media (hover: hover) and (pointer: fine)': {
			'&:hover': !disabled && {
				borderColor: theme.palette['graphics-gray'],
			},
		},

		...(isActive && {
			borderColor: theme.palette['graphics-accent'],
		}),
	})
);

export const ColorPickerWrapper = styled('div')<ColorPickerWrapperProps>(
	({ theme, disabled }) => ({
		display: 'flex',
		justifyContent: 'space-between',
		gap: theme.spaces.m,
		opacity: disabled
			? DEFAULT_DISABLED_INPUT_COMPONENT_OPACITY
			: DEFAULT_OPACITY,
		flexWrap: 'wrap',
	})
);

export const RandomColorsWrapper = styled('div')<RandomColorsWrapperProps>(
	({ flexible, spaceBetween, theme }) => ({
		display: flexible ? 'flex' : 'grid',
		flex: 1,
		flexWrap: 'wrap',
		justifyContent: spaceBetween ? 'space-between' : 'flex-start',
		alignItems: 'center',
		gap: spaceBetween ? RANDOM_COLOR_GAP : theme.spaces.m,
		minWidth: COLOR_PICKER_MIN_WIDTH,
	})
);

export const getColorPickerPopoverStyles = (theme: Theme) => ({
	background: theme.palette['background-white'],
	borderRadius: BorderRadiuses.m,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	boxShadow: theme.namedShadows.dropDownListWrapper,
	padding: theme.spaces.m,
});
