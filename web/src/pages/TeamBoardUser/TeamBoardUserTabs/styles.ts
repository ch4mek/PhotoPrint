import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

export const TabsContainer = styled('div')(({ theme }) => ({
	width: '100%',
	height: '100%',
	display: 'flex',
	flexDirection: 'column',
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderRadius: BorderRadiuses.m,
	backgroundColor: theme.palette['background-white'],
}));

export const TabsContentWrapper = styled('div')(({ theme }) => ({
	width: '100%',
	height: '100%',
	flexGrow: 1,
	minHeight: 0,
	display: 'flex',
	flexDirection: 'column',
	padding: theme.spaces.m,
	paddingTop: theme.spaces.s,
}));
