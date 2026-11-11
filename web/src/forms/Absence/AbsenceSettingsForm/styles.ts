import styled from '@emotion/styled';

export const CounterWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.m,
	alignItems: 'center',
	justifyContent: 'space-between',
	marginBottom: theme.spaces.l,
}));
