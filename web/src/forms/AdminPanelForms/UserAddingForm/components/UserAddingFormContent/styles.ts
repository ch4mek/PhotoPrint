import { css } from '@emotion/react';
import styled from '@emotion/styled';

import { popUpBreakpoints } from '@/constants/popUp';

export const FormWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.l,
	width: '100%',

	[theme.breakpoints.down(popUpBreakpoints.tabletXS)]: {
		flexDirection: 'column',
	},
}));

export const FormColumn = styled('div')(
	({ theme }) => css`
		display: flex;
		flex-direction: column;
		gap: ${theme.spaces.l}px;
		width: 100%;
	`
);
