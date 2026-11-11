import styled from '@emotion/styled';

export const ChipsCellWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.xs,
	flexWrap: 'wrap',
	alignItems: 'flex-start',
	width: '100%',
}));
