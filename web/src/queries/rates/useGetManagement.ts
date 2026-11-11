import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import { useRef } from 'react';

import { ratesQueriesKeys } from '@/constants/queriesKeys';
import { getManagementsApiEndpoint } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { toNullableNumber, toNullableString } from '@/helpers/nullable';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { getManagementSortField } from '@/pages/ProfitAndLoss/components/ManagementTab/constants';
import { ServerResponseError } from '@/queries/types';
import { GetManagementResponse } from '@/types/management';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import {
	ManagementColumnStatisticsCache,
	ManagementMainStatisticsCache,
	UseGetManagementParams,
} from './types';
import {
	getManagementStatisticsFlags,
	mergeManagementStatisticsResponse,
} from './utils/managementStatisticsCache';

export const useGetManagement = (
	{
		top,
		page,
		order,
		orderBy,
		year,
		month,
		filters,
		allDepartmentsLength,
	}: UseGetManagementParams,
	options?: PatchedQueryOptions<
		GetManagementResponse,
		AxiosError<ServerResponseError>
	>
) => {
	const { id, user, department, project, client, legalEntity } = filters;
	const mainStatisticsCacheRef = useRef<ManagementMainStatisticsCache | null>(
		null
	);
	const columnStatisticsCacheRef =
		useRef<ManagementColumnStatisticsCache | null>(null);

	return useQueryWithCallback<
		GetManagementResponse,
		AxiosError<ServerResponseError>
	>({
		queryKey: [
			ratesQueriesKeys.getManagement,
			page,
			top,
			order,
			orderBy,
			year,
			month,
			id,
			user,
			department,
			project,
			client,
			legalEntity,
			allDepartmentsLength,
		],
		queryFn: async () => {
			try {
				const departmentIds =
					!department.length || department.length === allDepartmentsLength
						? null
						: department;

				const filtersKey = JSON.stringify(filters);

				const {
					shouldCalculateMainStatistics,
					shouldCalculateColumnStatistics,
				} = getManagementStatisticsFlags({
					year,
					month,
					filtersKey,
					mainStatisticsCache: mainStatisticsCacheRef.current,
					columnStatisticsCache: columnStatisticsCacheRef.current,
				});

				const { data } = await axios.get<GetManagementResponse>(
					getManagementsApiEndpoint(year, month, top, page + 1),
					{
						params: {
							UserId: toNullableNumber(toNullableString(id)),
							UserFullName: toNullableString(user),
							ProjectName: toNullableString(project),
							ClientName: toNullableString(client),
							ClientLegalEntity: toNullableString(legalEntity),
							DepartmentIds: departmentIds,
							OrderingField: getManagementSortField(orderBy),
							IsDescendingOrdering: order === SortDirection.DESC,
							ShouldCalculateMainStatistics: shouldCalculateMainStatistics,
							ShouldCalculateColumnStatistics: shouldCalculateColumnStatistics,
						},
						paramsSerializer: {
							indexes: null,
						},
					}
				);

				const { response, mainStatisticsCache, columnStatisticsCache } =
					mergeManagementStatisticsResponse({
						data,
						year,
						month,
						filtersKey,
						shouldCalculateMainStatistics,
						shouldCalculateColumnStatistics,
						mainStatisticsCache: mainStatisticsCacheRef.current,
						columnStatisticsCache: columnStatisticsCacheRef.current,
					});

				mainStatisticsCacheRef.current = mainStatisticsCache;
				columnStatisticsCacheRef.current = columnStatisticsCache;

				return response;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		refetchOnMount: true,
		...options,
	});
};
