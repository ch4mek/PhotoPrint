import { UseQueryResult } from '@tanstack/react-query';
import axios, { AxiosResponse } from 'axios';
import buildQuery from 'odata-query';

import { PageableListOfItems } from '../types';

import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { TimesheetQueryKeys } from '@/constants/queriesKeys';
import { GET_TIMESHEET_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import dayjs from '@/lib/dayjs';

import { TimesheetData, TimesheetWithBlockedDays } from './types';

const TIMESHEET_RECORDS_AMOUNT_PER_PAGE = 100000;

export const useGetTimesheet = (
	userId: number,
	startDate: Date,
	endDate: Date,
	clientId?: number | string,
	projectId?: number | string,
	managedByPMO?: boolean,
	isPersonalMode?: boolean
): UseQueryResult<TimesheetWithBlockedDays, unknown> =>
	useQueryWithCallback<TimesheetWithBlockedDays, unknown>({
		queryKey: [
			TimesheetQueryKeys.userTimesheet,
			userId,
			dayjs(startDate).startOf('day').format(YYYY_MM_DD_DAYJS_DATE_FORMAT),
			dayjs(endDate).format(YYYY_MM_DD_DAYJS_DATE_FORMAT),
		],
		queryFn: () => {
			const formattedStartDate = dayjs(startDate)
				.startOf('day')
				.format(YYYY_MM_DD_DAYJS_DATE_FORMAT);
			const formattedEndDate = dayjs(endDate).format(
				YYYY_MM_DD_DAYJS_DATE_FORMAT
			);

			const reportForDateFilter = `reportForDate ge ${formattedStartDate} and reportForDate le ${formattedEndDate}`;

			const query = buildQuery({
				filter: {
					and: [
						reportForDateFilter,
						clientId && `clientId eq ${clientId}`,
						projectId && `projectId eq ${projectId}`,
					],
				},
			});

			return axios
				.get(
					`${GET_TIMESHEET_API_ENDPOINT}/${userId}${query}&skip=${0}&top=${TIMESHEET_RECORDS_AMOUNT_PER_PAGE}`,
					{
						params: {
							onlyManagedProjects: managedByPMO,
							isPersonalMode: !!isPersonalMode,
						},
					}
				)
				.then((res: AxiosResponse<PageableListOfItems<TimesheetData>>) => ({
					...res.data.pageableListOfItems,
					blockedDays: res.data.blockedDays,
					items: res.data.pageableListOfItems.items,
				}));
		},
		refetchOnWindowFocus: false,
	});
