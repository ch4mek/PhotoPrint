import styled from '@emotion/styled';

export const CategoryControlsWrapper = styled('div')(({ theme }) => ({
	width: '100%',
	display: 'flex',
	gap: theme.spaces.s,

	padding: theme.spaces.m,
	paddingBottom: 0,
}));
