import { css } from '@emotion/react';
import styled from '@emotion/styled';

export const TimeTrackReportNotificationBody = styled('div')(
	({ theme }) => css`
		display: flex;
		justify-content: space-between;
		flex-direction: column;
		gap: ${theme.spaces.s}px;
		cursor: pointer;
	`
);

export const TimeTrackReportNotificationInfoContainer = styled('div')(
	({ theme }) => css`
		display: grid;
		grid-template-columns: min-content minmax(0px, 1fr);
		column-gap: ${theme.spaces.s}px;
		align-items: center;
	`
);

export const TimeTrackReportFileDescriptionContainer = styled('div')(
	({ theme }) => css`
		display: flex;
		flex-direction: column;
		gap: ${theme.spaces.xs}px;
		min-width: 0;
		word-wrap: break-word;
		overflow-wrap: break-word;
	`
);
