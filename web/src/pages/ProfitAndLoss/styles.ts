import styled from '@emotion/styled';

export const PageHeaderWrapper = styled('div')(({ theme }) => ({
	paddingBlock: theme.spaces.s,
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'space-between',
	gap: theme.spaces.s,

	[theme.breakpoints.down('sm')]: {
		flexDirection: 'column',
		alignItems: 'flex-start',
	},
}));

export const HeaderInnerBlockWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,

	[theme.breakpoints.down('sm')]: {
		width: '100%',
		justifyContent: 'space-between',
	},
}));
