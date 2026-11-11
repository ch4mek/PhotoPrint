import { getFullName } from '@/helpers/formatName';
import { ManagersPieChartResponse } from '@/types/pieChart';

import { minutesToHours } from './minutesToHours';

export function getDuration(arr?: Array<ManagersPieChartResponse>) {
	return minutesToHours(
		arr?.reduce((acc, value) => acc + value.trackedTime, 0)
	);
}

export function getValues(arr?: Array<ManagersPieChartResponse>) {
	return (
		arr?.map((item) => ({
			name: getFullName(item?.firstName, item?.lastName),
			value: item?.trackedTime,
			color: item?.color,
			firstName: item?.firstName,
			lastName: item?.lastName,
			totalTimeInMinutes: item?.trackedTime,
		})) || []
	);
}
