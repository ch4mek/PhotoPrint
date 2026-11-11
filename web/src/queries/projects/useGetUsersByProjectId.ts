import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { getLogicFilterValue } from '../utils/getLogicFilterValue';

import { ActivityStatus } from '@/constants/activityStatus';
import { BillabilityStatus } from '@/constants/billabilityStatus';
import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { UsersQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_PROJECTS_ENDPOINT } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { UserAccessFilters } from '@/forms/ProjectForm/types';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import dayjs from '@/lib/dayjs';

import { UseGetUsersByProjectIdResponse } from './types';

const formatFilterDate = (date: Date) =>
	dayjs(date).format(YYYY_MM_DD_DAYJS_DATE_FORMAT);

export const useGetUsersByProjectId = (
	projectId: number,
	page: number,
	rowsPerPage: number,
	order: SortDirection,
	orderBy: string,
	filters: UserAccessFilters
) => {
	const { userName, billability, status, endDate, startDate } = filters;

	return useQueryWithCallback<UseGetUsersByProjectIdResponse, AxiosError>({
		queryKey: [
			UsersQueriesKeys.usersByProject,
			`${page}`,
			`${rowsPerPage}`,
			`${projectId}`,
			order,
			orderBy,
			JSON.stringify(filters),
		],
		queryFn: async () => {
			const query = buildQuery({
				filter: {
					and: [
						`ProjectId eq ${projectId}`,
						userName &&
							`(contains(tolower(userFullName), tolower('${replaceSpecialCharacters(
								userName
							)}')))`,
						billability.length &&
							getLogicFilterValue(
								billability,
								BillabilityStatus.Billable,
								'IsTimeOnProjectBillable'
							),
						status &&
							getLogicFilterValue(status, ActivityStatus.Active, 'isActive'),
						startDate?.from &&
							`UserOnProjectFrom ge ${formatFilterDate(startDate.from)}`,
						startDate?.to &&
							`UserOnProjectFrom le ${formatFilterDate(startDate.to)}`,
						endDate?.from &&
							`UserOnProjectTo ge ${formatFilterDate(endDate.from)}`,
						endDate?.to && `UserOnProjectTo le ${formatFilterDate(endDate.to)}`,
					],
				},
				orderBy: `${orderBy} ${order}`,
				top: rowsPerPage,
				skip: page * rowsPerPage,
			});

			const { data } = await axios.get(`${GET_USER_PROJECTS_ENDPOINT}${query}`);

			return data;
		},
		refetchOnWindowFocus: false,
		enabled: Boolean(projectId),
	});
};
