import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';
import { List } from 'react-virtualized';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { scrollbarWidths } from '@/constants/commonStyles';
import { ASSIGNMENT_FORM_MOBILE_BREAKPOINT } from '@/forms/Absence/AbsenceAssignmentForm/constants';

const MIN_LIST_WIDTH = 200;
const MOBILE_LIST_HEIGHT = 310;
const DESKTOP_LIST_HEIGHT = 470;

export const ListWrapper = styled('div')(({ theme }) => ({
	flex: 1,
	borderRadius: BorderRadiuses.m,
	padding: `${theme.spaces.m}px 0`,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
}));

export const ListContent = styled('div')(({ theme }) => ({
	minWidth: MIN_LIST_WIDTH,
	height: DESKTOP_LIST_HEIGHT,

	[theme.breakpoints.down(ASSIGNMENT_FORM_MOBILE_BREAKPOINT)]: {
		height: MOBILE_LIST_HEIGHT,
		minWidth: 'none',
	},
}));

export const ListItem = styled('div')(({ theme }) => ({
	paddingBottom: theme.spaces.s,
}));

export const VirtualizedListWrapper = styled(List as any)(({ theme }) => ({
	padding: `0 ${theme.spaces.m}px`,
	...getScrollbarStyles(theme, scrollbarWidths.s, 'vertical'),
}));
