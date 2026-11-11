import { css } from '@emotion/react';
import styled from '@emotion/styled';

import { popUpBreakpoints } from '@/constants/popUp';

const INPUT_MAX_WIDTH = 350;

export const DateAndTypeInputsWrapper = styled('div')(
	({ theme }) => css`
		display: grid;
		gap: ${theme.spaces.s}px;
		grid-template-columns: repeat(2, 1fr);
	`
);

export const FirstInputsWrapper = styled('div')(({ theme }) => ({
	maxWidth: INPUT_MAX_WIDTH,
	width: '100%',
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,
}));

export const LastInputsWrapper = styled('div')(
	({ theme }) => css`
		display: grid;
		column-gap: ${theme.spaces.s}px;
		row-gap: ${theme.spaces.l}px;
		grid-template-columns: repeat(2, minmax(${INPUT_MAX_WIDTH}px, 1fr));

		${theme.breakpoints.down(popUpBreakpoints.tabletL)} {
			max-width: ${INPUT_MAX_WIDTH}px;
			grid-template-columns: 1fr;
		}
	`
);
