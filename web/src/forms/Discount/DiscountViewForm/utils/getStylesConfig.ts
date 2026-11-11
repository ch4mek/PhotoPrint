import { Theme } from '@emotion/react';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';
import { CSSProperties } from 'react';

import { scrollbarWidths } from '@/constants/commonStyles';
import {
	BORDER_HEIGHT,
	POPUP_HEADER_HEIGHT,
	popUpSizes,
	VERTICAL_PADDINGS,
} from '@/constants/popUp';
import {
	MIDDLE_BREAKPOINT,
	POPUP_CONTENT_HEIGHT,
} from '@/forms/Discount/DiscountViewForm/constants';

const VERTICAL_PADDING = 48;

const getHeightData = (
	viewPortHeight: number,
	defaultHeight: number,
	windowWidth: number
) => {
	if (viewPortHeight < defaultHeight || windowWidth < MIDDLE_BREAKPOINT) {
		return {
			maxHeight: viewPortHeight,
			height: '100%',
		};
	}

	return {
		maxHeight: defaultHeight,
		height: defaultHeight,
		minHeight: `calc(${POPUP_CONTENT_HEIGHT + VERTICAL_PADDING}px)`,
		overflowY: 'auto',
	};
};

export const getPopUpStylesConfig = (
	theme: Theme,
	sectionHeight: number,
	windowHeight: number,
	windowWidth: number
) => {
	const viewPortHeight =
		windowHeight - VERTICAL_PADDINGS - POPUP_HEADER_HEIGHT - BORDER_HEIGHT;
	const defaultHeight = sectionHeight + VERTICAL_PADDING;
	const isPopupShorterThanViewport = viewPortHeight < defaultHeight;

	return {
		container: {
			width: popUpSizes.m,

			[theme.breakpoints.down(MIDDLE_BREAKPOINT)]: {
				height: '100%',

				'& > div': {
					height: `calc(100% - ${POPUP_HEADER_HEIGHT}px)`,
				},
			},
		},
		content: {
			...getScrollbarStyles(theme, scrollbarWidths.s, 'vertical'),
			...getHeightData(viewPortHeight, defaultHeight, windowWidth),

			...(isPopupShorterThanViewport && {
				paddingBottom: theme.spaces.l,
			}),

			[theme.breakpoints.up(MIDDLE_BREAKPOINT)]: {
				padding: !isPopupShorterThanViewport
					? `${theme.spaces.l}px 0 0 ${theme.spaces.l}px`
					: `${theme.spaces.l}px 0 ${theme.spaces.l}px ${theme.spaces.l}px`,
				overflowY: 'auto',
			},
		} as CSSProperties,
	};
};
