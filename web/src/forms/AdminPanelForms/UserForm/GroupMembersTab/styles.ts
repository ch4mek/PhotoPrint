import styled from '@emotion/styled';

export const GroupMembersTabWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
	height: '100%',
	minHeight: 0,
}));
