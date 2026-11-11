import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';

import { scrollbarWidths } from '@/constants/commonStyles';

export const CategoryListWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,

	padding: theme.spaces.m,
	paddingTop: 0,
	marginTop: theme.spaces.m,

	...getScrollbarStyles(theme, scrollbarWidths.m, 'vertical'),
}));
