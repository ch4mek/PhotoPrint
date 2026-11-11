import styled from '@emotion/styled';

export const ChipsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'start',
	gap: theme.spaces.m,

	[`${theme.breakpoints.down(theme.breakpoints.tabletS)}`]: {
		gap: theme.spaces.l,
		flexDirection: 'column',
	},
}));

export const AutoselectWrapper = styled('div')(({ theme }) => ({
	width: '50%',
	flex: 1,
	display: 'flex',
	flexDirection: 'column',

	[`${theme.breakpoints.down(theme.breakpoints.tabletS)}`]: {
		width: '100%',
	},
}));
