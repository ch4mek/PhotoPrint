import { useEffect, Dispatch, SetStateAction } from 'react';

import { DateFormat } from '@/constants/date';
import { filterUniqueWorkplaces } from '@/helpers/filterUniqueWorkplaces';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { useGetReservation } from '@/queries/booking';
import { Workplace } from '@/types/booking';
import { OfficeType } from '@/types/configuration';

export const useFilledDesksWithRangeDate = (
	firstDay: Dayjs,
	lastDay: Dayjs,
	setWorkplaces: Dispatch<SetStateAction<Workplace[]>>,
	error: string | null,
	office: OfficeType,
	workspaceId: number | undefined,
	enabled: boolean
) => {
	const { timeZone } = office;
	const { data, dataUpdatedAt, isFetching, isLoading } = useGetReservation({
		fromDate: dayjs(firstDay)
			.clone()
			.tz(timeZone)
			.hour(0)
			.minute(0)
			.second(0)
			.millisecond(0)
			.format(DateFormat.ISO),
		toDate: dayjs(lastDay)
			.clone()
			.tz(timeZone)
			.hour(0)
			.minute(0)
			.second(0)
			.millisecond(0)
			.format(DateFormat.ISO),
		error,
		officeId: office.id,
		workspaceId,
		enabled,
	});

	useEffect(() => {
		if (data) {
			const newWorkplaces = data.reduce(
				(acc: Workplace[], dataObj) => [...acc, ...dataObj.userWorkplaces],
				[]
			);
			const filteredUniqueWrokplaces = filterUniqueWorkplaces(newWorkplaces);

			setWorkplaces(filteredUniqueWrokplaces);
		}
	}, [dataUpdatedAt, data]);

	const showDesks = !isFetching && !isLoading;

	return { showDesks };
};
