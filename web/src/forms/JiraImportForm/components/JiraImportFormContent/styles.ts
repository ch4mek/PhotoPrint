import styled from '@emotion/styled';

export const FormWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,
}));
