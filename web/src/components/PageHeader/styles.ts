import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { Link } from 'react-router-dom';

import { HeaderContainerProps } from './types';

export const BackButtonLink = styled(Link)(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'center',
	color: theme.palette['text-black'],
	textDecoration: 'none',
	borderRadius: theme.spaces.xs,
	marginRight: theme.spaces.s,

	'&:hover': {
		backgroundColor: theme.palette['hover-base-2'],
		color: theme.palette['graphics-primary-2'],
	},
}));

const commonStyles = css({
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'start',
});

export const HeaderContainer = styled('div')<HeaderContainerProps>(
	({ theme, isWithTitle, needWrapDownDesktopXS }) => [
		commonStyles,
		{
			height: 'fit-content',
			padding: `${isWithTitle ? theme.spaces.m : theme.spaces.s}px 0px`,

			...(needWrapDownDesktopXS && {
				[theme.breakpoints.down(theme.breakpoints.desktopXS)]: {
					flexDirection: 'column',
					gap: theme.spaces.m,
					alignItems: 'flex-start',
					width: '100%',
				},
			}),
		},
	]
);

export const TitleRowWrapper = styled('div')({
	display: 'flex',
	alignItems: 'center',
});

export const TitleWrapper = styled('div')({
	whiteSpace: 'normal',
});

export const HeaderControlsContainer = styled('div')<{
	needWrapDownDesktopXS: boolean;
}>(({ theme, needWrapDownDesktopXS }) => [
	commonStyles,
	{
		justifyContent: 'flex-end',
		flex: '1 1 auto',
		columnGap: theme.spaces.s,

		...(needWrapDownDesktopXS && {
			[theme.breakpoints.down(theme.breakpoints.desktopXS)]: {
				width: '100%',
			},
		}),
	},
]);

export const AsideWrapper = styled('div')(({ theme }) => ({
	marginLeft: theme.spaces.m,
}));
