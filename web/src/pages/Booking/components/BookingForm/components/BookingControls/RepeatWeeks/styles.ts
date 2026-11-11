import styled from '@emotion/styled';

export const Wrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	paddingBottom: theme.spacing(2),
	gap: theme.spaces.s,
	userSelect: 'none',
	color: theme.palette['text-black'],
}));
