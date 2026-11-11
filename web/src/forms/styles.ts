import { css } from '@emotion/react';
import styled from '@emotion/styled';

export const FormWrapper = styled('div')(
	({ theme }) => css`
		display: flex;
		flex-direction: column;
		gap: ${theme.spaces.l}px;
		width: 100%;
	`
);
