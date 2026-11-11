import { useMemo } from 'react';

import dayjs from '@/lib/dayjs';
import { useGetUserTimesheetStatus } from '@/queries/timesheet/useGetUserTimesheetStatus';

export function useGetUserTimeSheetStatusOnMonthIntersect(
	currentWeek: Date[],
	month: number,
	year: number,
	userId: number,
	isWeekFilter: boolean,
	queryOptions: Parameters<typeof useGetUserTimesheetStatus>['3']
) {
	const secondMonthDate = useMemo<[number, number]>(() => {
		if (isWeekFilter) {
			const firstDate = dayjs(currentWeek[0]);
			const secondDate = dayjs(currentWeek.at(-1));

			if (firstDate.month() + 1 !== secondDate.month() + 1) {
				return [secondDate.month() + 1, secondDate.year()];
			}
		}

		return [0, 0];
	}, [currentWeek, isWeekFilter]);

	const { data } = useGetUserTimesheetStatus(
		userId,
		month,
		year,
		queryOptions,
		secondMonthDate
	);

	return data?.status;
}
