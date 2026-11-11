import { Theme } from '@emotion/react';

export const ABSENCES_WIDGET_HEIGHT = 194;
export const ABSENCES_WIDGET_WITH_CAROUSEL = ABSENCES_WIDGET_HEIGHT + 12;

export const EMPTY_STATE_ICON_HEIGHT = 96;

const MAX_ITEM_WIDTH = 320;
export const MAX_SINGLE_ITEM_WIDTH = 256;
export const MAX_SLIDES_TO_SHOW = 2;

export const getCarouselSlideStyles = (theme: Theme) => ({
	maxWidth: MAX_ITEM_WIDTH,
	minWidth: 0,
	padding: `0 ${theme.spaces.s}px`,
});

export const getWidgetContentStyles = (theme: Theme) => ({
	padding: `0 ${theme.spaces.s}px ${theme.spaces.m}px`,
});
