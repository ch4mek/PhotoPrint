import DocViewer from '@cyntler/react-doc-viewer';
import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';
import { breakpoints } from '@mms/mms-ui-library/theme';

import { scrollbarWidths } from '@/constants/commonStyles';
import { zIndexes } from '@/constants/zIndexes';
import { BLACK } from '@/theme';

import { VIEWER_HEADER_HEIGHT } from './constants';
import { DocViewerHeaderPosition } from './types';

const VIEWER_LAYOUT_BACKGROUND = `${BLACK}80`;
const VIEWER_WIDTH = {
	desktop: '50%',
	desktopS: '70%',
	tablet: '80%',
	tabletS: '95%',
};

export const Overlay = styled('div')({
	position: 'fixed',
	top: 0,
	left: 0,
	height: '100vh',
	width: '100vw',
	backgroundColor: VIEWER_LAYOUT_BACKGROUND,
	zIndex: zIndexes.filterPopup,
});

export const ViewerLayout = styled('div')({
	width: '100%',
	height: '100%',
	display: 'flex',
	flexDirection: 'column',
});

export const ViewerBody = styled('div')(({ theme }) => ({
	height: `calc(100vh - ${VIEWER_HEADER_HEIGHT}px)`,
	width: VIEWER_WIDTH.desktop,
	margin: `0 auto`,
	paddingTop: theme.spaces.m,
	paddingBottom: theme.spaces.m,

	[theme.breakpoints.down(breakpoints.desktopS)]: {
		width: VIEWER_WIDTH.desktopS,
	},

	[theme.breakpoints.down(breakpoints.tablet)]: {
		width: VIEWER_WIDTH.tablet,
	},

	[theme.breakpoints.down(breakpoints.tabletS)]: {
		width: VIEWER_WIDTH.tabletS,
	},
}));

export const SpinnerContainer = styled('div')(({ theme }) => ({
	width: '100%',
	height: `calc(100vh - ${VIEWER_HEADER_HEIGHT}px - 2 * ${theme.spaces.m}px)`,
	display: 'flex',
	justifyContent: 'center',
	alignItems: 'center',
}));

export const DocumentViewer = styled(DocViewer)<{
	headerPosition: DocViewerHeaderPosition;
}>(({ theme, headerPosition: { left, right, top } }) => ({
	...getScrollbarStyles(theme, scrollbarWidths.m, 'both'),

	'& #proxy-renderer': {
		display: 'flex',
		height: '100%',
	},

	'& #pdf-download': {
		display: 'none',
	},

	'& #pdf-controls': {
		position: 'fixed',
		top,
		left,
		right,
		padding: theme.spaces.s,
		zIndex: zIndexes.header,
	},
}));
