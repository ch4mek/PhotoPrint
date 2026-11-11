import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

export const BillableWorkingHoursHeaderWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'center',
	padding: `${theme.spaces.s}px ${theme.spaces.m}px`,
	border: `1px solid ${theme.palette['graphics-input-stroke']}`,
	borderBottom: 0,
	backgroundColor: theme.palette['background-white'],
	color: theme.palette['text-black'],
	borderRadius: `${BorderRadiuses.m}px ${BorderRadiuses.m}px 0 0`,
}));
