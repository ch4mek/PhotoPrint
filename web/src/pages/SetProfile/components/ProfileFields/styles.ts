import { styled } from '@mui/material/styles';

export const Wrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,
	width: '100%',
}));
