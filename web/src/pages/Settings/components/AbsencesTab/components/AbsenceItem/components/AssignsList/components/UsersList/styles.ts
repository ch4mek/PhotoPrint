import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

export const AssignUserCardList = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
}));

export const AssignUserCard = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
	backgroundColor: theme.palette['background-base-2'],
	padding: theme.spaces.s,
	borderRadius: BorderRadiuses.m,
}));

export const FetchNextPageLoader = styled('div')({
	display: 'flex',
	justifyContent: 'center',
	alignItems: 'center',
});
