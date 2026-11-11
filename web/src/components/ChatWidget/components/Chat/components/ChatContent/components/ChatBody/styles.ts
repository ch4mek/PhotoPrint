import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';

import { scrollbarWidths } from '@/constants/commonStyles';

export const ChatBodyWrapper = styled('ul')(({ theme }) => ({
	flex: 1,
	display: 'flex',
	flexDirection: 'column',

	gap: theme.spaces.s,
	padding: `0 ${theme.spaces.m}px`,
	margin: 0,

	...getScrollbarStyles(theme, scrollbarWidths.m, 'vertical'),
}));
