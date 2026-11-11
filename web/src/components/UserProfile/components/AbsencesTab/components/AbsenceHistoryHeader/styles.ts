import styled from '@emotion/styled';

export const AbsenceHistoryHeaderWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'center',
	marginBottom: theme.spaces.m,
	gap: theme.spaces.s,
	flexWrap: 'wrap',
}));
