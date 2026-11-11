import styled from '@emotion/styled';
import { Scrollbar } from '@mms/mms-ui-library/components/Scrollbar';

import { SCROLL_BOTTOM_MARGIN } from '../main/constants';

import { zIndexes } from '@/constants/zIndexes';

import { HEADER_HEIGHT, TOP_POSITION_LOAD_WRAPPER } from './constants';

export const AppWrapper = styled('div')(({ theme }) => ({
	height: '100%',
	paddingTop: HEADER_HEIGHT,
	backgroundColor: theme.palette['background-base-1'],
}));

export const MainContent = styled('main')({
	padding: 0,
	position: 'relative',
	flexGrow: 1,
	height: '100%',
});

export const CustomScroll = styled(Scrollbar)({
	width: '100%',
	height: '100%',
	scrollMarginBottom: SCROLL_BOTTOM_MARGIN,
});

export const LoaderWrapper = styled('div')({
	position: 'absolute',
	width: '100%',
	top: TOP_POSITION_LOAD_WRAPPER,
	left: 0,
	zIndex: zIndexes.linearLoader,
});
