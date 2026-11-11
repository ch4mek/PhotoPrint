import styled from '@emotion/styled';

export const UserAccessTabWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
	height: '100%',
	minHeight: 0,
}));

export const UserAccesssTabHeader = styled('div')({
	display: 'flex',
	flexDirection: 'column',
	alignItems: 'flex-end',
	flexShrink: 0,
});

export const UserAccessGridWrapper = styled('div')({
	flex: '1 1 0',
	minHeight: 0,
});
