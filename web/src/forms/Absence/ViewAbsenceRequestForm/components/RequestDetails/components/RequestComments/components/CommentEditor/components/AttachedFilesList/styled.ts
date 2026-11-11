import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

export const SquareAttachedFilesWrapper = styled('div')(({ theme }) => ({
	padding: `${theme.spaces.s}px`,
	display: 'flex',
	flexWrap: 'wrap',
	gap: `${theme.spaces.s}px`,
	borderTopLeftRadius: BorderRadiuses.m,
	borderTopRightRadius: BorderRadiuses.m,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderBottom: 'none',
}));
