import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

export const ReportsStatisticWrapper = styled('div')(({ theme }) => ({
	backgroundColor: theme.palette['background-white'],
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderBottom: 'none',
	borderRadius: `${BorderRadiuses.m}px ${BorderRadiuses.m}px 0 0`,
	padding: `${theme.spaces.m}px ${theme.spaces.m}px 0`,
}));
