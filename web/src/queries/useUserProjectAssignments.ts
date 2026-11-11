import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ActivityStatus } from '@/constants/activityStatus';
import { BillabilityStatus } from '@/constants/billabilityStatus';
import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_PROJECTS_ENDPOINT } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { convertOrderToQueryParam } from '@/helpers/convertOrderToQueryParam';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import dayjs from '@/lib/dayjs';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { ProjectsTableFilters } from '@/types/projectsTableFilters';
import { UserProjectLookup } from '@/types/userProjectLookup';

import { ByPageResponse } from './types';
import { getLogicFilterValue } from './utils/getLogicFilterValue';

const paramsToReverseOrder = ['isActive', 'isTimeOnProjectBillable'];

export const useUserProjectAssignments = (
	userId: string | number | undefined,
	page: number,
	rowsPerPage: number,
	filters: ProjectsTableFilters,
	order?: SortDirection,
	orderByCondition?: string,
	options?: PatchedQueryOptions<ByPageResponse<UserProjectLookup>, AxiosError>
) => {
	const { projectName, billability, isActive, endDate, startDate, clientName } =
		filters;

	return useQueryWithCallback<ByPageResponse<UserProjectLookup>, AxiosError>({
		queryKey: [
			TeamboardQueriesKeys.projects,
			`${page}`,
			`${rowsPerPage}`,
			`${userId}`,
			JSON.stringify(filters),
			order,
			orderByCondition,
		],
		queryFn: async () => {
			const query = buildQuery({
				filter: {
					and: [
						userId && `userId eq ${userId}`,
						projectName &&
							`(contains(tolower(projectName), tolower('${replaceSpecialCharacters(
								projectName
							)}')))`,
						clientName &&
							`(contains(tolower(clientName), tolower('${replaceSpecialCharacters(
								clientName
							)}')))`,
						billability.length &&
							getLogicFilterValue(
								billability,
								BillabilityStatus.Billable,
								'isTimeOnProjectBillable'
							),
						isActive.length &&
							getLogicFilterValue(isActive, ActivityStatus.Active, 'isActive'),
						startDate?.to &&
							`date(userOnProjectFrom) le ${dayjs(startDate?.to).format(
								YYYY_MM_DD_DAYJS_DATE_FORMAT
							)}`,
						startDate?.from &&
							`date(userOnProjectFrom) ge ${dayjs(startDate?.from).format(
								YYYY_MM_DD_DAYJS_DATE_FORMAT
							)}`,
						endDate?.to &&
							`date(userOnProjectTo) le ${dayjs(endDate?.to).format(
								YYYY_MM_DD_DAYJS_DATE_FORMAT
							)}`,
						endDate?.from &&
							`date(userOnProjectTo) ge ${dayjs(endDate?.from).format(
								YYYY_MM_DD_DAYJS_DATE_FORMAT
							)}`,
					],
				},
				orderBy:
					order && orderByCondition
						? `${orderByCondition} ${convertOrderToQueryParam(
								order,
								orderByCondition,
								paramsToReverseOrder
						  )}`
						: undefined,
				top: rowsPerPage,
				skip: page * rowsPerPage,
			});
			const { data } = await axios.get<ByPageResponse<UserProjectLookup>>(
				`${GET_USER_PROJECTS_ENDPOINT}${query}`,
				{
					params: {
						userId,
					},
				}
			);

			return data;
		},
		...options,
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
		enabled: Boolean(userId),
	});
};
