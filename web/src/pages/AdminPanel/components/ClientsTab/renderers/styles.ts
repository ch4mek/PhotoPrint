import styled from '@emotion/styled';

export const CellWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	alignItems: 'center',
	width: '100%',
	justifyContent: 'space-between',
}));
