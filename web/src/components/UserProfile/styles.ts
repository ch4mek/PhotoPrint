import { css } from '@emotion/react';
import styled from '@emotion/styled';

export const UserDataWrapper = styled('div')(
	({ theme }) => css`
		display: flex;
		flex-direction: column;
		gap: ${theme.spaces.m}px;
	`
);

export const ContentForm = styled('div')(
	({ theme }) => css`
		display: flex;
		flex-direction: column;
		flex: auto;
		gap: ${theme.spaces.m}px;
		width: 100%;
	`
);
