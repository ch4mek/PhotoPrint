import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const FORM_WIDTH = 232;

export const PopoverContentWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	alignItems: 'flex-end',
	gap: theme.spaces.m,

	width: FORM_WIDTH,
	padding: theme.spaces.m,

	border: `1px solid ${theme.palette['graphics-input-stroke']}`,
	backgroundColor: theme.palette['background-white'],
	borderRadius: BorderRadiuses.m,
	boxShadow: theme.namedShadows.dropDownListWrapper,
}));
