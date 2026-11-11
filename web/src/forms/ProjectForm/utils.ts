import { Theme } from '@emotion/react';

import {
	BORDER_HEIGHT,
	MOBILE_POPUP_BUTTONS_CONTAINER_HEIGHT,
	POPUP_BUTTONS_CONTAINER_HEIGHT,
	POPUP_HEADER_HEIGHT,
	POPUP_TABS_HEIGHT,
	VERTICAL_PADDINGS,
	popUpBreakpoints,
	popUpPaddings,
	popUpSizes,
} from '@/constants/popUp';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';

import { MIN_HEIGHT_PROJECT_FORM_POPUP, ProjectFromTabId } from './constants';

const PROJECT_FORM_POPUP_HEIGHT_TABLET = 566;

const getControlsContainerHeight = (windowWidth: number) =>
	windowWidth < popUpBreakpoints.tabletXS
		? MOBILE_POPUP_BUTTONS_CONTAINER_HEIGHT
		: POPUP_BUTTONS_CONTAINER_HEIGHT;

const getAdaptivePopupMaxHeight = (windowHeight: number) =>
	windowHeight - VERTICAL_PADDINGS;

const getContentVerticalPadding = (activeTab: ProjectFromTabId, theme: Theme) =>
	activeTab === ProjectFromTabId.USER_ACCESS
		? theme.spaces.s + theme.spaces.l
		: theme.spaces.l + theme.spaces.m;

const getContentMaxHeightPx = (
	popupHeight: number,
	windowHeight: number,
	hasControls: boolean
) => {
	const controlsHeight = hasControls ? POPUP_BUTTONS_CONTAINER_HEIGHT : 0;

	const viewportMaxHeight =
		windowHeight -
		POPUP_TABS_HEIGHT -
		controlsHeight -
		VERTICAL_PADDINGS -
		POPUP_HEADER_HEIGHT -
		BORDER_HEIGHT;

	const containerMaxHeight =
		popupHeight -
		POPUP_HEADER_HEIGHT -
		POPUP_TABS_HEIGHT -
		BORDER_HEIGHT -
		controlsHeight;

	return Math.min(viewportMaxHeight, containerMaxHeight);
};

const getProjectFormPopupHeight = (
	windowHeight: number,
	windowWidth: number
) => {
	const adaptivePopupMaxHeight = getAdaptivePopupMaxHeight(windowHeight);

	if (windowWidth >= popUpBreakpoints.tablet) {
		return adaptivePopupMaxHeight;
	}

	return Math.min(PROJECT_FORM_POPUP_HEIGHT_TABLET, adaptivePopupMaxHeight);
};

export const getTabWrapperHeight = ({
	theme,
	activeTab,
	windowHeight,
	windowWidth,
}: {
	theme: Theme;
	activeTab: ProjectFromTabId;
	windowHeight: number;
	windowWidth: number;
}) => {
	if (windowWidth >= popUpBreakpoints.tablet) {
		if (activeTab === ProjectFromTabId.PROJECT) {
			return (
				MIN_HEIGHT_PROJECT_FORM_POPUP -
				getControlsContainerHeight(windowWidth) -
				theme.spaces.s
			);
		}

		return MIN_HEIGHT_PROJECT_FORM_POPUP;
	}

	const hasControls = activeTab === ProjectFromTabId.PROJECT;
	const popupHeight = getProjectFormPopupHeight(windowHeight, windowWidth);
	const contentMaxHeight = getContentMaxHeightPx(
		popupHeight,
		windowHeight,
		hasControls
	);

	return contentMaxHeight - getContentVerticalPadding(activeTab, theme);
};

export const getPopUpStylesConfig = (
	theme: Theme,
	activeTab: ProjectFromTabId,
	windowHeight: number,
	windowWidth: number
) => {
	const hasControls = activeTab === ProjectFromTabId.PROJECT;
	const isTabletWidth = windowWidth < popUpBreakpoints.tablet;
	const popupHeight = getProjectFormPopupHeight(windowHeight, windowWidth);
	const contentMaxHeight = getContentMaxHeightPx(
		popupHeight,
		windowHeight,
		hasControls
	);

	return {
		container: {
			width: popUpSizes.m,

			[theme.breakpoints.down(popUpBreakpoints.tablet)]: {
				width: popUpSizes.s,

				...(isTabletWidth && {
					height: popupHeight,
					maxHeight: getAdaptivePopupMaxHeight(windowHeight),
				}),
			},
		},
		content: {
			...getAdaptivePopUpContentStylesConfig({
				theme,
				hasTabs: true,
				hasContols: hasControls,
				windowHeight,
				windowWidth,
			}),
			maxHeight: `${contentMaxHeight}px`,

			padding:
				activeTab === ProjectFromTabId.USER_ACCESS
					? popUpPaddings.projectsTab
					: popUpPaddings.hasControls,
		},
	};
};
