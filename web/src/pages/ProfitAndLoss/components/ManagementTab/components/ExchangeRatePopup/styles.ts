import styled from '@emotion/styled';

export const ContentWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
}));

export const CurrenciesWrapper = styled(ContentWrapper)(({ theme }) => ({
	gap: theme.spaces.s,
}));
