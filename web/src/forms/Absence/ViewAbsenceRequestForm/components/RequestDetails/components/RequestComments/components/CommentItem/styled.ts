import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

export const CommentWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
	padding: `${theme.spaces.s}px`,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderRadius: BorderRadiuses.m,
}));
