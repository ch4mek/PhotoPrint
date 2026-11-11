import { Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const COLOR_PICKER_SIZE = 16;

export const CircleColorPickerWrapper = styled('div')(() => ({}));

export const CircleColorPickerButton = styled('button')<{
	color: string;
	disabled: boolean;
}>(({ color, disabled }) => ({
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'center',

	cursor: disabled ? 'default' : 'pointer',

	padding: 0,
	width: COLOR_PICKER_SIZE,
	height: COLOR_PICKER_SIZE,

	border: 'none',
	borderRadius: BorderRadiuses.half,

	background: color || 'transparent',
}));

export const getCircleColorPickerPopoverStyles = (theme: Theme) => ({
	background: theme.palette['background-white'],
	borderRadius: BorderRadiuses.m,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	boxShadow: theme.namedShadows.dropDownListWrapper,
	padding: theme.spaces.m,
});
