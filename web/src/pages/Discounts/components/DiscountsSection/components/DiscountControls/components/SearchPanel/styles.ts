import styled from '@emotion/styled';

export const SearchPanelWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	marginLeft: 'auto',
	flex: 1,
	justifyContent: 'flex-end',
}));
