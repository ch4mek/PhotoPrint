import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';
import { Tag } from '@mms/mms-ui-library/components/Tag';
import { lightTheme } from '@mms/mms-ui-library/theme';

import {
	TIMESHEET_TOTAL_COLUMN_WIDTH,
	TIMESHEET_USER_NAME_COLUMN_WIDTH,
	TIMESHEET_USER_NAME_COLUMN_WIDTH_MOBILE,
} from '../constants';
import { getTimesheetStatusColor } from '../utils/getTimesheetStatusColor';

import {
	getTotalColumnBackgroundColor,
	getTaskInfoColumnBackgroundColor,
} from '@/helpers/getColor';

const headerStatusPositionStyles = {
	position: 'absolute',
	left: 106,
	top: 18,
} as const;

export const getTimesheetHorizontalScrollbarStyles = (theme: Theme) => css`
	::-webkit-scrollbar {
		border-right: 1px solid ${getTotalColumnBackgroundColor(theme)};
	}

	::-webkit-scrollbar-track {
		background: ${getTaskInfoColumnBackgroundColor(theme)};
		border-left: ${TIMESHEET_USER_NAME_COLUMN_WIDTH}px solid
			${getTaskInfoColumnBackgroundColor(theme)};
		border-right: ${TIMESHEET_TOTAL_COLUMN_WIDTH}px solid
			${getTotalColumnBackgroundColor(theme)};
	}

	::-webkit-scrollbar-track-piece:horizontal {
		margin-left: ${TIMESHEET_USER_NAME_COLUMN_WIDTH}px;
	}

	@media (max-width: ${theme.breakpoints.tablet}px) and (min-width: ${theme
			.breakpoints.tabletXS}px) {
		::-webkit-scrollbar-track-piece:horizontal {
			margin-left: ${TIMESHEET_USER_NAME_COLUMN_WIDTH_MOBILE}px;
		}
	}

	::-webkit-scrollbar-track-piece:horizontal:end {
		margin-right: ${TIMESHEET_TOTAL_COLUMN_WIDTH}px;
	}

	::-webkit-scrollbar-corner {
		border-radius: 0;
		background-color: ${getTotalColumnBackgroundColor(theme)};
	}
`;

export const TimesheetStatus = styled(Tag)<{
	variant: string;
	isHeaderStatus?: boolean;
}>(
	({ theme, variant, isHeaderStatus }) =>
		css`
			border: none;
			color: ${lightTheme.palette['text-black']};

			&,
			&:hover {
				background-color: ${getTimesheetStatusColor(theme, variant)};
			}

			${isHeaderStatus && css(headerStatusPositionStyles)}
		`
);
