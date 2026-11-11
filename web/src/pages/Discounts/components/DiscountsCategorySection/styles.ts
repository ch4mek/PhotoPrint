import styled from '@emotion/styled';

import { discountBreakpoints } from '@/constants/discounts';
import { SectionWrapper } from '@/pages/Discounts/styles';

const SECTION_WIDTH = 278;

export const DiscountsCategorySectionWrapper = styled(SectionWrapper)(
	({ theme }) => ({
		display: 'flex',
		flexDirection: 'column',
		width: SECTION_WIDTH,
		minWidth: SECTION_WIDTH,
		maxWidth: SECTION_WIDTH,

		[theme.breakpoints.down(discountBreakpoints.desktopL)]: {
			border: 'none',
			borderRadius: 0,
			flex: 1,
			overflow: 'auto',
		},
	})
);
