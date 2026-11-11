import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';

import { scrollbarWidths } from '@/constants/commonStyles';
import { MOBILE_DISCOUNT_BREAKPOINT } from '@/pages/Discounts/constants';
import { SectionWrapper } from '@/pages/Discounts/styles';

export const DiscountsSectionWrapper = styled(SectionWrapper)(({ theme }) => ({
	flex: 1,

	display: 'flex',
	flexDirection: 'column',
	overflow: 'auto',

	[theme.breakpoints.down(MOBILE_DISCOUNT_BREAKPOINT)]: {
		minWidth: `calc(100vw - ${theme.spaces.xl}px)`,
	},

	...getScrollbarStyles(theme, scrollbarWidths.m, 'horizontal'),
}));
