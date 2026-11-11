import { Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const CONTENT_WIDTH = 228;

export const getColorPickerPopoverStyles = (theme: Theme) => ({
	background: theme.palette['background-white'],
	borderRadius: BorderRadiuses.m,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	boxShadow: theme.namedShadows.dropDownListWrapper,
	padding: theme.spaces.m,
});

export const getColorPickerPopoverContentStyles = () => ({
	width: CONTENT_WIDTH,
});

export const ColorPickerWrapper = styled('div')({});
