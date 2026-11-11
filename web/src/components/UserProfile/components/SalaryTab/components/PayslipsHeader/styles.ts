import styled from '@emotion/styled';

export const PayslipsHeaderWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'center',
	marginBottom: theme.spaces.s,
	gap: theme.spaces.s,
	flexWrap: 'wrap',
	flexShrink: 0,
}));
