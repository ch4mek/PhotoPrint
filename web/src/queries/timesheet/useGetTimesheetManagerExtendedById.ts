import { keepPreviousData, UseQueryResult } from '@tanstack/react-query';
import axios from 'axios';
import buildQuery from 'odata-query';

import { GET_TIMESHEET_API_ENDPOINT } from '@/constants/queryPaths';
import { Roles } from '@/constants/roles';
import { parseDayjsDateToUTCString } from '@/helpers/date';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import dayjs, { Dayjs } from '@/lib/dayjs';

import {
	TimesheetManagerDataById,
	TimesheetManagerDataByIdResponse,
} from './types';

export const useGetTimesheetManagerExtendedById = (
	id: number,
	startDate: Date,
	endDate: Date,
	role: string | Roles,
	clientId: number | string,
	projectId: number | string,
	month: number | string | Dayjs,
	periodFilter: string
): UseQueryResult<TimesheetManagerDataById[], unknown> =>
	useQueryWithCallback<TimesheetManagerDataById[], unknown>({
		queryKey: [
			'timesheet-manager-id',
			id,
			startDate.getTime(),
			endDate.getTime(),
			role,
			clientId,
			projectId,
			month,
			periodFilter,
		],
		queryFn: async () => {
			const dayjsStartDate = dayjs(startDate).startOf('day');
			const dayjsEndDate = dayjs(endDate).hour(23).minute(59).second(59);

			const fromString = parseDayjsDateToUTCString(dayjsStartDate);
			const toString = parseDayjsDateToUTCString(dayjsEndDate);

			const query = buildQuery({
				filter: {
					and: [
						`cast(reportForDate, Edm.DateTimeOffset) ge ${fromString} and cast(reportForDate, Edm.DateTimeOffset) le ${toString}`,
						{
							or: [
								clientId
									? `contains(cast(clientId, Edm.String), cast(${clientId}, Edm.String))`
									: undefined,
								projectId
									? `contains(cast(projectId, Edm.String), cast(${projectId}, Edm.String))`
									: ``,
							],
						},
					],
				},
				top: 100,
				skip: 0,
			});

			const { data } = await axios.get<TimesheetManagerDataByIdResponse>(
				`${GET_TIMESHEET_API_ENDPOINT}/${id}${query}`
			);

			return data.pageableListOfItems.items;
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
