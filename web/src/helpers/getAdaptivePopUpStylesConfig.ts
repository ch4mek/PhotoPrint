import { Theme } from '@emotion/react';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';

import { scrollbarWidths } from '@/constants/commonStyles';
import {
	POPUP_TABS_HEIGHT,
	POPUP_BUTTONS_CONTAINER_HEIGHT,
	POPUP_HEADER_HEIGHT,
	VERTICAL_PADDINGS,
	BORDER_HEIGHT,
	MOBILE_POPUP_BUTTONS_CONTAINER_HEIGHT,
	popUpBreakpoints,
} from '@/constants/popUp';

interface GetAdaptivePopUpContentStylesConfigProps {
	theme: Theme;
	hasTabs?: boolean;
	hasContols?: boolean;
	windowHeight: number;
	windowWidth?: number;
	isFixPopUpMaxHeight?: boolean;
}

export const getAdaptivePopUpContentStylesConfig = ({
	hasContols,
	hasTabs,
	theme,
	windowHeight,
	windowWidth = 0,
	isFixPopUpMaxHeight = false,
}: GetAdaptivePopUpContentStylesConfigProps) => {
	const tabsHeight = hasTabs ? POPUP_TABS_HEIGHT : 0;
	const controlsContainerHeight =
		windowWidth < popUpBreakpoints.tabletXS
			? MOBILE_POPUP_BUTTONS_CONTAINER_HEIGHT
			: POPUP_BUTTONS_CONTAINER_HEIGHT;
	const controlsHeight = hasContols ? controlsContainerHeight : 0;

	const heightCalculation = `calc(${windowHeight}px - ${
		tabsHeight +
		controlsHeight +
		VERTICAL_PADDINGS +
		POPUP_HEADER_HEIGHT +
		BORDER_HEIGHT
	}px)`;

	return {
		...getScrollbarStyles(theme, scrollbarWidths.s, 'vertical'),
		...(isFixPopUpMaxHeight && { height: heightCalculation }),
		maxHeight: heightCalculation,
	};
};
