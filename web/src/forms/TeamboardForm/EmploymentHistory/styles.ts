import styled from '@emotion/styled';

export const FormRow = styled('div')(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: 'repeat(2, 1fr)',
	width: '100%',
	gap: theme.spaces.s,
}));
