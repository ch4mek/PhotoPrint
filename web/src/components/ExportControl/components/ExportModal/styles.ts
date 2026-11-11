import { css } from '@emotion/react';
import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

export const SelectionModalWrapper = styled('div')({
	display: 'flex',
	flexDirection: 'column',
});

export const SelectsSections = styled('div')(({ theme }) => ({
	paddingBlock: theme.spaces.m,
	paddingLeft: theme.spaces.m,
	borderBottom: `1px solid ${theme.palette['graphics-divider-1']}`,
}));

export const OptionsSection = styled('div')(
	({ theme }) => css`
		padding: ${theme.spaces.s}px;
		display: flex;
		flex-grow: 1;
		flex-direction: column;
	`
);

export const Option = styled('div')(({ theme }) => ({
	display: 'flex',
	padding: theme.spaces.s,
	paddingRight: theme.spaces.m,
	cursor: 'pointer',
	borderRadius: BorderRadiuses.m,

	'&:hover': {
		background: theme.palette['hover-base-2'],
	},
}));
