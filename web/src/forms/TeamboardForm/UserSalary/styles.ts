import { css } from '@emotion/react';
import styled from '@emotion/styled';

const INPUT_WIDTH = 171;

export const FormContent = styled('div')(
	({ theme }) => css`
		width: 100%;
		display: grid;
		gap: ${theme.spaces.s}px;
		grid-template-columns: repeat(2, minmax(0, ${INPUT_WIDTH}px));
	`
);
