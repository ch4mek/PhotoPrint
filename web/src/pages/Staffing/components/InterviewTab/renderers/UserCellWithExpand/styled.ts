import styled from '@emotion/styled';

export const CellWrapper = styled.div(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.xs,
	justifyContent: 'space-between',
	height: '100%',
	width: '100%',
}));

export const UserText = styled.span({
	overflow: 'hidden',
	textOverflow: 'ellipsis',
	whiteSpace: 'nowrap',
});
