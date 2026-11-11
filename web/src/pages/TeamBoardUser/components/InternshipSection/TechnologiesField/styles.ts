import styled from '@emotion/styled';

export const TechnologiesList = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	alignItems: 'flex-start',
	gap: theme.spaces.s,
	width: '100%',
}));
