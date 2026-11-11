import { css } from '@emotion/react';
import styled from '@emotion/styled';

export const ReportGeneratingFailureNotificationBody = styled('div')(
	({ theme }) => css`
		display: flex;
		align-items: center;
		gap: ${theme.spaces.xs}px;
	`
);
