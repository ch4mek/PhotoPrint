import styled from '@emotion/styled';

export const SelectedSeatViewWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	marginBottom: theme.spaces.m,
	color: theme.palette['text-black'],
}));
