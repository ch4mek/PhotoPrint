import { Theme } from '@emotion/react';
import styled from '@emotion/styled';

const HORIZONTAL_ROW_HEIGHT = 1;

export const CustomHorizontalRow = styled('hr')<{
	color: keyof Theme['palette'];
	margin?: string;
}>(({ theme, color, margin }) => ({
	height: HORIZONTAL_ROW_HEIGHT,
	border: 'none',
	color: theme.palette[color],
	margin: margin || 0,
	backgroundColor: theme.palette[color],
}));
