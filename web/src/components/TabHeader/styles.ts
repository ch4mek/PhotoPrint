import { css } from '@emotion/react';
import styled from '@emotion/styled';

export const TabHeaderWrapper = styled.div(
	({ theme }) => css`
		display: flex;
		width: 100%;
		justify-content: flex-end;
		margin-bottom: ${theme.spaces.s}px;
	`
);
