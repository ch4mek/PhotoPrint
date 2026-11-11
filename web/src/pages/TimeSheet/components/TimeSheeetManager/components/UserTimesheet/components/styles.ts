import { Interpolation } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { PLAIN_TEXT_FONT_SIZE } from '@/constants/commonStyles';
import { filterByPeriod } from '@/constants/filters';
import {
	TIMESHEET_EXPANDER,
	TIMESHEET_STATUS,
	TIMESHEET_USER_CELL_NAME,
} from '@/constants/gridAreaNames';
import { createActiveBorderColor } from '@/helpers/getColor';
import { ColumnTotalTypographyProps } from '@/pages/TimeSheet/components/TimesheetEngineer/components/TotalDayTimeCell.tsx/types';
import {
	NORMAL_WORK_DAY_IN_MINUTES,
	TIMESHEET_CLOSED_ROW_HEIGHT,
	TIMESHEET_MANAGER_ROW_HEIGHT_MOBILE,
} from '@/pages/TimeSheet/components/TimesheetEngineer/constants';
import { getHoverColumnIndexByOrder } from '@/pages/TimeSheet/utils/getHoverColumnIndexByOrder';

const INDENTATION_BETWEEN_TAG_AND_USERNAME = 2;

export const DayTotalTimeWrapper = styled('div')<{
	column: number;
	displayRightBorder: boolean;
}>(({ column, displayRightBorder }) => {
	const styles: Interpolation<undefined> = {
		padding: '0px 10px',
		display: 'flex',
		fontSize: '14px',
		alignItems: 'center',
		justifyContent: 'center',
		gridColumn: column,
		flexDirection: 'column',
		gridRow: 1,
	};

	if (displayRightBorder) {
		styles.borderRight = 'none';
	}

	return styles;
});

export const DayTotalTimeTypography = styled(
	Typography
)<ColumnTotalTypographyProps>(({ theme, trackedInMinutes }) => {
	let color: string = theme.palette['base-4'];

	if (trackedInMinutes) {
		color = theme.palette['feedback-danger'];

		if (trackedInMinutes === NORMAL_WORK_DAY_IN_MINUTES) {
			color = theme.palette['feedback-success'];
		}

		if (trackedInMinutes > NORMAL_WORK_DAY_IN_MINUTES) {
			color = theme.palette.trackedTimeCell?.info ?? color;
		}
	}

	return {
		color,
	};
});

export const PeriodTotalTimeWrapper = styled('div')<{ periodLength: number }>(
	({ theme, periodLength }) => ({
		borderLeft: `1px solid ${createActiveBorderColor(theme)}`,
		position: 'sticky',
		gridColumn: `${getHoverColumnIndexByOrder(periodLength)} / ${
			getHoverColumnIndexByOrder(periodLength) + 1
		}`,
		gridRow: 1,
		right: '-0.1px',
	})
);

export const CellTextWrapper = styled('div')({
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'center',
	flexDirection: 'column',
	width: '100%',
});

export const UserInfoWrapper = styled('div')(({ theme }) => ({
	maxWidth: '100%',
	boxSizing: 'border-box',
	gridColumn: 2,
	gridRow: 1,
	position: 'sticky',
	insetInlineStart: '0px',
	borderRight: `1px solid ${createActiveBorderColor(theme)}`,
}));

export const UserInfoContentWrapper = styled('div')(({ theme }) => ({
	maxWidth: '100%',
	height: '100%',
	padding: `0 ${theme.spaces.s}px`,
}));

export const UserInfoContent = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	alignItems: 'center',
	minWidth: 0,
	justifyContent: 'space-between',
	flex: 1,
	fontSize: PLAIN_TEXT_FONT_SIZE,
	gridArea: 'name',

	[theme.breakpoints.down(theme.breakpoints.tablet)]: {
		paddingBottom: INDENTATION_BETWEEN_TAG_AND_USERNAME,
	},
}));

export const UserInfoNameCellWrapper = styled('div')<{
	shouldStatusRender: boolean;
	periodFilter: string;
}>(({ theme, shouldStatusRender, periodFilter }) => ({
	display: 'grid',
	justifyContent: 'space-between',
	alignItems: 'center',
	gridTemplateAreas: `'${TIMESHEET_USER_CELL_NAME} ${TIMESHEET_STATUS} ${TIMESHEET_EXPANDER}'`,
	gridTemplateColumns: '1fr',
	gridTemplateRows: '3fr 1fr 1fr',

	[theme.breakpoints.down(theme.breakpoints.desktopXS)]: {
		gridTemplateAreas: `'${TIMESHEET_USER_CELL_NAME} ${TIMESHEET_EXPANDER}' '${TIMESHEET_STATUS} ${TIMESHEET_STATUS}'`,
		gridTemplateRows: '1fr 1fr',
		gridTemplateColumns: '3fr 1fr',
		lineHeight: shouldStatusRender
			? theme.typography.body['m-400'].lineHeight
			: 'auto',
		paddingBottom: shouldStatusRender ? theme.spaces.xs : 0,
		paddingTop: shouldStatusRender ? theme.spaces.xs : 0,
		height:
			periodFilter === filterByPeriod.monthFilter
				? TIMESHEET_MANAGER_ROW_HEIGHT_MOBILE
				: TIMESHEET_CLOSED_ROW_HEIGHT,
	},
}));
