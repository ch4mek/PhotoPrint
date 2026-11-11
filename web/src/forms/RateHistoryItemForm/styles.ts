import styled from '@emotion/styled';

export const RateHistoryItemFormFieldsRow = styled('div')(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: 'repeat(2, minmax(0, 1fr))',
	gap: theme.spaces.s,
	width: '100%',
}));
