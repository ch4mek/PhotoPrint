import styled from '@emotion/styled';

import { BLACK } from '../../theme';

import { HALF_OPACITY } from '@/constants/opacities';
import { zIndexes } from '@/constants/zIndexes';
import { addOpacityToColor } from '@/helpers/addOpacityToColor';

const HEADER_HEIGHT = 54;

export const MobileSidebarWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',

	backgroundColor: theme.palette['background-white'],
	height: '100%',

	maxHeight: `calc(100vh - ${HEADER_HEIGHT}px)`,
}));

export const ControlWrapper = styled('div')(({ theme }) => ({
	margin: `${theme.spaces.m}px 0 0 ${theme.spaces.m}px`,
}));

export const LayoutWrapper = styled('div')<{ activeShadow: boolean }>(
	({ activeShadow }) => ({
		position: 'fixed',
		top: HEADER_HEIGHT,
		left: 0,
		zIndex: zIndexes.mobileSidebar,
		width: '100%',
		height: '100%',
		boxSizing: 'border-box',

		backgroundColor: activeShadow
			? addOpacityToColor(BLACK, HALF_OPACITY)
			: 'none',
		display: 'flex',
		justifyContent: 'flex-start',
	})
);
