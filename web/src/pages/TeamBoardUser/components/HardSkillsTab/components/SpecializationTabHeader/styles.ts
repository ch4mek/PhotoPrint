import { css } from '@emotion/react';
import styled from '@emotion/styled';

import { DEFAULT_CONTROL_HEIGHT } from '@/constants/commonStyles';

export const TabHeaderWrapper = styled.div(
	({ theme }) => css`
		display: flex;
		align-items: center;
		width: 100%;
		justify-content: flex-end;
		margin-bottom: ${theme.spaces.s}px;
		gap: ${theme.spaces.s}px;
	`
);

export const EntryButtonWrapper = styled.div(
	({ theme }) => css`
		button {
			padding: ${theme.spaces.s}px ${theme.spaces.l}px;
			height: ${DEFAULT_CONTROL_HEIGHT}px;
		}
	`
);

export const SpecializationButtonWrapper = styled.div(
	({ theme }) => css`
		button {
			padding: ${theme.spaces.s}px ${theme.spaces.m}px;
		}
	`
);
