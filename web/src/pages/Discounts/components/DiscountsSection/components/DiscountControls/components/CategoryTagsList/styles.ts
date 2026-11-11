import styled from '@emotion/styled';

import { getScrollableControlStyles } from '@/pages/Discounts/components/DiscountsSection/components/DiscountControls/styles';
import { MOBILE_DISCOUNT_BREAKPOINT } from '@/pages/Discounts/constants';

export const CategoryTagsListWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	flexWrap: 'wrap',

	flex: 1,

	[theme.breakpoints.down(MOBILE_DISCOUNT_BREAKPOINT)]: {
		flexWrap: 'nowrap',
		...getScrollableControlStyles(),
	},
}));
