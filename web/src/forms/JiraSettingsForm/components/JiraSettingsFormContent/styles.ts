import styled from '@emotion/styled';

const DIVIDER_WIDTH = 10;

export const FormRow = styled('div')(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: `1fr ${DIVIDER_WIDTH}px 1fr`,
	alignItems: 'center',
	width: '100%',
	gap: theme.spaces.s,
}));
