import styled from '@emotion/styled';

export const PopUpBody = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
}));
