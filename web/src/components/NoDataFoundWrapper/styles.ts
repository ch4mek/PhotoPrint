import styled from '@emotion/styled';

export const NoDataFoundWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.xs,
	justifyContent: 'center',
	alignItems: 'center',
	height: '100%',
}));
