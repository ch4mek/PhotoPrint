import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { scrollbarWidths } from '@/constants/commonStyles';
import { HEADER_HEIGHT } from '@/layouts/mainWithoutSidebar/constants';

import {
	MOBILE_SIDEBAR_MENU_WIDTH,
	SIDEBAR_MENU_WIDTH,
	SIDEBAR_POPOVER_OFFSET,
} from './constants';
import { SidebarMenuWrapperProps } from './types';

export const SidebarMenuWrapper = styled.div<SidebarMenuWrapperProps>(
	({ theme, displayModules, windowHeight, isMobileView }) => ({
		padding: isMobileView ? theme.spaces.m : theme.spaces.s,
		background: theme.palette['background-white'],
		borderRadius: BorderRadiuses.m,
		border: `1px solid ${theme.palette['graphics-input-stroke']}`,
		width: SIDEBAR_MENU_WIDTH,
		boxShadow: theme.namedShadows.headerPopover,
		display: 'flex',
		flexDirection: 'column',
		gap: theme.spaces.m,
		maxHeight: windowHeight - HEADER_HEIGHT - SIDEBAR_POPOVER_OFFSET[1],
		...getScrollbarStyles(theme, scrollbarWidths.s, 'vertical'),

		...(displayModules
			? {
					borderTopLeftRadius: 0,
					borderBottomLeftRadius: 0,
					borderLeft: 'none',
			  }
			: undefined),

		[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
			width: MOBILE_SIDEBAR_MENU_WIDTH,
		},
	})
);

export const OtherAppWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
}));
