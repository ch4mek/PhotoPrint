import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const MAX_WIDTH_PLATE_SECTION = 850;
const MIN_DESKTOP_XS_WIDTH = 500;

export const PlateSectionWrapper = styled.div(({ theme }) => ({
	padding: `${theme.spaces.m}px`,
	height: '100%',
	display: 'flex',
	justifyContent: 'center',
}));

export const PlateSection = styled('div')<{ isEditor: boolean }>(
	({ isEditor, theme }) => ({
		display: 'flex',
		flexDirection: 'column',
		alignItems: 'center',
		width: '100%',
		maxWidth: isEditor ? '100%' : `${MAX_WIDTH_PLATE_SECTION}px`,
		backgroundColor: isEditor
			? theme.palette['background-base-1']
			: theme.palette['background-white'],
		border: `1px solid ${theme.palette['graphics-divider-1']}`,
		borderRadius: `${BorderRadiuses.m}px`,
		height: '100%',

		...(!isEditor && {
			[theme.breakpoints.down(theme.breakpoints.desktopXS)]: {
				minWidth: MIN_DESKTOP_XS_WIDTH,
			},

			[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
				minWidth: `calc(100vw - ${theme.spaces.xl}px)`,
			},
		}),
	})
);
