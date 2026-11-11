import { Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { SELECTION_BOX_Z_INDEX, SELECTION_COLORS } from './constants';

const BORDER_WIDTH = 1;

const getBackgroundColor = (
	theme: Theme,
	isOverlapped: boolean,
	isLoading: boolean
) => {
	if (isLoading) {
		return `${theme.palette['background-base-2']}80`;
	}

	return isOverlapped
		? SELECTION_COLORS['danger-bg']
		: SELECTION_COLORS['success-bg'];
};

const getBorderColor = (
	theme: Theme,
	isOverlapped: boolean,
	isLoading: boolean
) => {
	if (isLoading) {
		return theme.palette['graphics-primary-2'];
	}

	return isOverlapped
		? theme.palette['feedback-danger']
		: theme.palette['feedback-success'];
};

export const SelectionBoxWrapper = styled('div')<{
	left: number;
	width: number;
	isOverlapped: boolean;
	isOutOfLimit: boolean;
	isLoading: boolean;
	timelineSectionsHeight: number;
	top: number;
}>(
	({
		theme,
		left,
		width,
		isOverlapped,
		isOutOfLimit,
		isLoading,
		timelineSectionsHeight,
		top,
	}) => ({
		position: 'absolute',
		top,
		height: timelineSectionsHeight,
		backgroundColor: getBackgroundColor(theme, isOverlapped, isLoading),
		borderWidth: BORDER_WIDTH,
		borderStyle: 'solid',
		borderColor: getBorderColor(theme, isOverlapped, isLoading),
		borderRightWidth: isOutOfLimit ? 0 : BORDER_WIDTH,
		zIndex: SELECTION_BOX_Z_INDEX,
		left,
		width,
		cursor: 'pointer',
		pointerEvents: 'none',
	})
);
