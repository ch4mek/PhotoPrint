import { css } from '@emotion/react';
import styled from '@emotion/styled';

export const UpcomingReviewWrapper = styled('div')(
	({ theme }) => css`
		display: flex;
		align-items: center;
		gap: ${theme.spaces.m}px;
		color: ${theme.palette['text-black']};
	`
);
