import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { convertFilterArrayToQueryParam } from '../utils';
import { convertDateFromToValueToQueryParam } from '../utils/convertFromToValueToQueryParam';
import { getLogicFilterValue } from '../utils/getLogicFilterValue';

import { ActivityStatus } from '@/constants/activityStatus';
import { projectExitQueriesKeys } from '@/constants/queriesKeys';
import { GET_EXITS_API_ENDPOINT } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { convertOrderToQueryParam } from '@/helpers/convertOrderToQueryParam';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ProjectExitsFilters } from '@/pages/Staffing/components/ProjectExitsTab/types';
import { convertOrderByQueryParam } from '@/pages/Staffing/components/ProjectExitsTab/utils';
import { ByPageResponse } from '@/queries/types';
import { ProjectExitEntity } from '@/types/projectExits';

const paramsToReverseOrder = ['isActive'];

export const useGetProjectExits = (
	page: number,
	rowsPerPage: number,
	order: SortDirection,
	orderByCondition: string,
	filters: ProjectExitsFilters,
	allDepartmentsLength: number
) => {
	const {
		idFilter,
		userFilter,
		departmentFilter,
		groupFilter,
		clientFilter,
		projectFilter,
		closingDateFilter,
		closingReasonFilter,
		statusFilter,
	} = filters;

	return useQueryWithCallback<ByPageResponse<ProjectExitEntity>, AxiosError>({
		queryKey: [
			projectExitQueriesKeys.getProjectExits,
			page,
			rowsPerPage,
			order,
			orderByCondition,
			projectFilter,
			clientFilter,
			userFilter,
			departmentFilter,
			groupFilter,
			statusFilter,
			idFilter,
			closingDateFilter,
			closingReasonFilter,
		],
		queryFn: async () => {
			const query = buildQuery({
				filter: {
					and: [
						idFilter?.trim() &&
							`cast(id, Edm.String) eq cast('${replaceSpecialCharacters(
								idFilter.trim()
							)}', Edm.String)`,
						userFilter?.trim() &&
							`contains(tolower(fullName), tolower('${replaceSpecialCharacters(
								userFilter.trim()
							)}'))`,
						departmentFilter?.length && {
							departmentId: convertFilterArrayToQueryParam(
								departmentFilter,
								allDepartmentsLength
							),
						},
						groupFilter?.trim() &&
							`contains(tolower(userResourceGroupName), tolower('${replaceSpecialCharacters(
								groupFilter.trim()
							)}'))`,
						projectFilter?.trim() &&
							`contains(tolower(ProjectName), tolower('${replaceSpecialCharacters(
								projectFilter
							)}'))`,
						clientFilter?.trim() &&
							`contains(tolower(ClientName), tolower('${replaceSpecialCharacters(
								clientFilter
							)}'))`,
						...convertDateFromToValueToQueryParam(
							closingDateFilter,
							'closingDate'
						),
						closingReasonFilter &&
							`contains(tolower(ClosingReasonName), tolower('${replaceSpecialCharacters(
								closingReasonFilter
							)}'))`,
						statusFilter &&
							getLogicFilterValue(
								statusFilter,
								ActivityStatus.Active,
								'isActive'
							),
					],
				},
				orderBy: `${convertOrderByQueryParam(
					orderByCondition
				)} ${convertOrderToQueryParam(
					order,
					orderByCondition,
					paramsToReverseOrder
				)}`,
				top: rowsPerPage,
				skip: (page - 1) * rowsPerPage,
			});

			return axios
				.get<ByPageResponse<ProjectExitEntity>>(
					`${GET_EXITS_API_ENDPOINT}/${query}`
				)
				.then((res) => res.data);
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
};
