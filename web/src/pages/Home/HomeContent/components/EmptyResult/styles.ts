import styled from '@emotion/styled';

export const EmptyResultWrapper = styled('div')(({ theme }) => ({
	width: '100%',
	height: '100%',
	display: 'flex',
	justifyContent: 'center',
	alignItems: 'center',
	flexDirection: 'column',
	gap: theme.spaces.s,
}));
