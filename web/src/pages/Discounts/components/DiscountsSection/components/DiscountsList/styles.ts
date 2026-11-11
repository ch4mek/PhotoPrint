import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';

import { scrollbarWidths } from '@/constants/commonStyles';
import { gridBreakpoints } from '@/pages/Discounts/components/DiscountsSection/components/DiscountsList/constants';
import { MOBILE_DISCOUNT_BREAKPOINT } from '@/pages/Discounts/constants';

const CARD_WIDTH = 246;
const MAX_CARD_WIDTH = 350;

export const DiscountsListWrapper = styled('div')(({ theme }) => ({
	flex: 1,
	padding: theme.spaces.m,
	paddingTop: 0,

	...getScrollbarStyles(theme, scrollbarWidths.m, 'vertical'),
}));

export const DiscountsCardsWrapper = styled('div')(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: `repeat(auto-fit, ${CARD_WIDTH}px)`,
	gridGap: theme.spaces.m,

	width: '100%',

	[theme.breakpoints.down(theme.breakpoints.desktopL)]: {
		gridTemplateColumns: `repeat(5, minmax(${CARD_WIDTH}px, 1fr))`,
	},

	[theme.breakpoints.down(gridBreakpoints.desktopL)]: {
		gridTemplateColumns: `repeat(4, minmax(${CARD_WIDTH}px, 1fr))`,
	},

	[theme.breakpoints.down(gridBreakpoints.desktopM)]: {
		gridTemplateColumns: `repeat(3, minmax(${CARD_WIDTH}px, 1fr))`,
	},

	[theme.breakpoints.down(gridBreakpoints.desktopS)]: {
		gridTemplateColumns: `repeat(2, minmax(${CARD_WIDTH}px, 1fr))`,
	},

	[theme.breakpoints.down(MOBILE_DISCOUNT_BREAKPOINT)]: {
		justifyContent: 'center',
		gridTemplateColumns: `repeat(auto-fit, minmax(${CARD_WIDTH}px, ${MAX_CARD_WIDTH}px))`,
	},
}));
