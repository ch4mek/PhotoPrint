import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

export const Wrapper = styled('div')(({ theme }) => ({
	position: 'relative',
	gridColumnStart: 2,
	alignSelf: 'start',
	display: 'flex',
	alignItems: 'center',
	flexWrap: 'wrap',
	justifyContent: 'center',
	padding: `${theme.spaces.s}px 0`,
	height: '100%',

	borderRadius: BorderRadiuses.m,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	backgroundColor: [theme.palette['background-white']],

	[theme.breakpoints.down(theme.breakpoints.desktopS)]: {
		gridColumnStart: 'auto',
		backgroundColor: [theme.palette['background-white']],
	},
}));
