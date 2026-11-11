import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

export const TableRowWrapper = styled('div')(({ theme }) => ({
	position: 'relative',
	border: `1px solid ${theme.palette['graphics-input-stroke']}`,
	backgroundColor: theme.palette['background-white'],
	borderRadius: BorderRadiuses.m,
	boxSizing: 'border-box',
	color: theme.palette['text-black'],
}));
