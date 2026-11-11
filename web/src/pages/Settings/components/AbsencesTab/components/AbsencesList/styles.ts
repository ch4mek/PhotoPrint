import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';

import { scrollbarWidths } from '@/constants/commonStyles';

export const AbsencesListWrapper = styled('ul')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,

	margin: 0,
	padding: `0 ${theme.spaces.m}px ${theme.spaces.m}px`,
	listStyleType: 'none',

	flex: '1 1 0%',
	...getScrollbarStyles(theme, scrollbarWidths.m, 'vertical'),

	[theme.breakpoints.up(theme.breakpoints.desktopXS)]: {
		marginBottom: theme.spaces.m,
		paddingBottom: theme.spaces.xs,
	},
}));
