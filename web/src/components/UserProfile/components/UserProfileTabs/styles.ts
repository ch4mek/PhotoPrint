import styled from '@emotion/styled';
import { Scrollbar } from '@mms/mms-ui-library/components/Scrollbar';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { POPUP_HEADER_HEIGHT, VERTICAL_PADDINGS } from '@/constants/popUp';

import { TabsContainerProps } from './types';

const UPCOMING_PERFOMANCE_REVIEW_HEIGHT = 20;
const PERSONAL_INFO_HEIGHT = 86;
const TABS_CONTAINER_MIN_HEIGHT = 280;
const POPUP_INNER_PADDINGS = 50;
const GAPS_COUNT = 2;

export const TabsContainer = styled('div')<TabsContainerProps>(
	({ theme, windowHeight, isHeightUnrestricted, minHeight }) => ({
		width: '100%',
		height: '100%',
		display: 'flex',
		flexDirection: 'column',
		border: `1px solid ${theme.palette['graphics-divider-1']}`,
		borderRadius: BorderRadiuses.m,
		backgroundColor: theme.palette['background-white'],
		minHeight: minHeight || TABS_CONTAINER_MIN_HEIGHT,
		maxHeight: isHeightUnrestricted
			? 'none'
			: windowHeight -
			  VERTICAL_PADDINGS -
			  POPUP_HEADER_HEIGHT -
			  UPCOMING_PERFOMANCE_REVIEW_HEIGHT -
			  PERSONAL_INFO_HEIGHT -
			  POPUP_INNER_PADDINGS -
			  GAPS_COUNT * theme.spaces.m,
	})
);

export const TabsContentWrapper = styled(Scrollbar)<{
	hasReducedTopPadding?: boolean;
}>(({ theme, hasReducedTopPadding = false }) => ({
	width: '100%',
	height: '100%',
	flexGrow: 1,
	minHeight: 0,
	display: 'flex',
	flexDirection: 'column',
	padding: theme.spaces.m,
	...(hasReducedTopPadding && { paddingTop: theme.spaces.s }),
}));
