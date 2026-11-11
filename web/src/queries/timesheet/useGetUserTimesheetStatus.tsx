import { UseQueryResult } from '@tanstack/react-query';
import axios from 'axios';

import { TimesheetQueryKeys } from '@/constants/queriesKeys';
import { getUserTimesheetStatusByMonthEndpointUrl } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { TimesheetStatusType } from '@/pages/TimeSheet/types';
import { getPreferrableTimesheetStatus } from '@/pages/TimeSheet/utils/getPreferrableTimesheetStatus';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

interface GetUserTimesheetStatusResponseType {
	userId: number;
	status: TimesheetStatusType | null;
}

async function timeSheetStatusRequest(
	userId: number,
	month: number,
	year: number
) {
	const { data } = await axios.get<GetUserTimesheetStatusResponseType>(
		`${getUserTimesheetStatusByMonthEndpointUrl(userId)}`,
		{
			params: {
				month,
				year,
			},
		}
	);

	return data;
}

export const useGetUserTimesheetStatus = (
	userId: number,
	month: number,
	year: number,
	options: PatchedQueryOptions<GetUserTimesheetStatusResponseType, Error>,
	secondData: [number, number]
): UseQueryResult<GetUserTimesheetStatusResponseType, Error> =>
	useQueryWithCallback<GetUserTimesheetStatusResponseType, Error>({
		...options,
		queryKey: [
			TimesheetQueryKeys.timesheetStatus,
			userId,
			month,
			year,
			secondData[0],
			secondData[1],
		],
		queryFn: async () => {
			const data = await timeSheetStatusRequest(userId, month, year);
			let secondMonthData;

			if (secondData[0] && secondData[1]) {
				secondMonthData = await timeSheetStatusRequest(
					userId,
					secondData[0],
					secondData[1]
				);
			}

			return {
				status: secondMonthData
					? getPreferrableTimesheetStatus(data.status, secondMonthData.status)
					: data.status,
				userId,
			};
		},
		refetchOnWindowFocus: false,
	});
