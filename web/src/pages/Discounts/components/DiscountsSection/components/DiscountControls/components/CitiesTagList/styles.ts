import styled from '@emotion/styled';

import { getScrollableControlStyles } from '@/pages/Discounts/components/DiscountsSection/components/DiscountControls/styles';
import { MOBILE_DISCOUNT_BREAKPOINT } from '@/pages/Discounts/constants';

const MAX_CITIES_WRAPPER_WIDTH = 446;

export const CitiesTagListWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,

	maxWidth: MAX_CITIES_WRAPPER_WIDTH,

	[theme.breakpoints.down(MOBILE_DISCOUNT_BREAKPOINT)]: {
		...getScrollableControlStyles(),
	},
}));
