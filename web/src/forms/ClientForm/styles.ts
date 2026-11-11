import styled from '@emotion/styled';

export const FormContentWrapper = styled('div')(({ theme }) => ({
	width: '100%',
	display: 'flex',
	flexDirection: 'column',
	justifyContent: 'center',
	gap: theme.spaces.l,
}));
