import groupBy from 'lodash/groupBy';
import { useMemo } from 'react';

import { TIMESHEET_DATE_FORMAT } from '../../TimesheetEngineer/constants';
import { TimeTrackItem } from '../../TimesheetEngineer/types';
import { TimeSheetManagerRow, TimeSheetManagerRowType } from '../types';

import { Roles } from '@/constants/roles';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { TimesheetManagerData } from '@/queries/timesheet/types';

export const useTimeSheetManagerRows = (
	data: TimesheetManagerData[] | undefined,
	dataUpdatedAt: Dayjs | Date | number,
	role: Roles,
	period: Date[]
) =>
	useMemo(() => {
		if (!data) {
			return [];
		}

		const rows: TimeSheetManagerRow[] = Object.values(data).map((item) => {
			const {
				userName,
				userId,
				color,
				timesheets,
				isActive,
				userDepartmentId,
				status = null,
				role: userRole,
			} = item;

			const groupedByReportDateTimesheets = groupBy(
				timesheets,
				(timesheet) => timesheet.reportForDate
			);

			const timeMap = new Map(
				Object.entries(groupedByReportDateTimesheets).map(
					([reportForDate, userTimesheets]) => {
						const [factTrackedTime, billableTrackedTime] =
							userTimesheets.reduce(
								(totalTime, userTimesheet) => {
									const {
										ftm: factTrackedInMinutes,
										btm: billableTrackedInMinutes,
									} = userTimesheet;

									const totalTimeCopy = totalTime;

									if (factTrackedInMinutes) {
										totalTimeCopy[0] += factTrackedInMinutes;
									}

									if (billableTrackedInMinutes) {
										totalTimeCopy[1] += billableTrackedInMinutes;
									}

									return totalTimeCopy;
								},
								[0, 0]
							);

						const dateStringRepresentation = dayjs(reportForDate).format(
							TIMESHEET_DATE_FORMAT
						);

						const timeTrackItem: TimeTrackItem = {
							trackId: userTimesheets[0].id,
							factTrackedInMinutes: factTrackedTime,
							billableTrackedInMinutes: billableTrackedTime,
							reportForDate,
							isLoading: {
								fact: false,
								billable: false,
							},
						};

						return [dateStringRepresentation, timeTrackItem];
					}
				)
			);

			return {
				userId,
				timeMap,
				color,
				userName,
				isActiveUser: isActive,
				userDepartmentId,
				role: userRole,
				type: TimeSheetManagerRowType.UserInfo,
				periodLength: period.length,
				isExpanded: false,
				shouldDisplaySkeleton: true,
				status,
			};
		});

		return rows;
	}, [data, dataUpdatedAt]);
