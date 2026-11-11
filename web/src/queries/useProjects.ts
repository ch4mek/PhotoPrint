import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ActivityStatus } from '@/constants/activityStatus';
import { ProjectsQueriesKeys } from '@/constants/queriesKeys';
import { GET_PROJECTS_API_ENDPOINT } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { convertOrderToQueryParam } from '@/helpers/convertOrderToQueryParam';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ProjectsFilters } from '@/pages/AdminPanel/components/ProjectsTab/types';
import { ByPageResponse } from '@/queries/types';
import { ProjectEntity } from '@/types/project';

import { getLogicFilterValue } from './utils/getLogicFilterValue';

const convertProjectOrderByToQueryParam = (orderBy: string) => {
	if (orderBy === 'isPrivate') {
		return 'isPublic';
	}

	return orderBy;
};

const paramsToReverseOrder = ['isActive', 'isPublic', 'isBillable'];

export const useProjects = (
	page: number,
	rowsPerPage: number,
	order: SortDirection,
	orderByCondition: string,
	filters: ProjectsFilters
) => {
	const {
		projectFilter,
		clientFilter,
		factTrackedTimeFilter: { from: factFrom, to: factTo },
		billableTrackedTimeFilter: { from: billableFrom, to: billableTo },
		billableFilter,
		statusFilter,
		publicFilter,
		idFilter,
	} = filters;

	return useQueryWithCallback<ByPageResponse<ProjectEntity>, AxiosError>({
		queryKey: [
			ProjectsQueriesKeys.projects,
			page,
			rowsPerPage,
			order,
			orderByCondition,
			projectFilter,
			clientFilter,
			factFrom,
			factTo,
			billableTo,
			billableFrom,
			statusFilter,
			publicFilter,
			idFilter,
			billableFilter,
		],
		queryFn: () => {
			const query = buildQuery({
				filter: {
					and: [
						projectFilter &&
							`contains(tolower(Name), tolower('${replaceSpecialCharacters(
								projectFilter
							)}'))`,
						clientFilter &&
							`contains(tolower(ClientName), tolower('${replaceSpecialCharacters(
								clientFilter
							)}'))`,
						factFrom &&
							`factTrackedTime ge ${replaceSpecialCharacters(
								String(+factFrom * 60)
							)}`,
						factTo &&
							`factTrackedTime le ${replaceSpecialCharacters(
								String(+factTo * 60)
							)}`,
						billableFrom &&
							`billableTrackedTime ge ${replaceSpecialCharacters(
								String(+billableFrom * 60)
							)}`,
						billableTo &&
							`billableTrackedTime le ${replaceSpecialCharacters(
								String(+billableTo * 60)
							)}`,
						statusFilter &&
							getLogicFilterValue(
								statusFilter,
								ActivityStatus.Active,
								'isActive'
							),
						publicFilter &&
							getLogicFilterValue(publicFilter, 'Yes', 'isPublic'),
						billableFilter &&
							getLogicFilterValue(billableFilter, 'Yes', 'isBillable'),
						idFilter &&
							`cast(id, Edm.String) eq cast('${replaceSpecialCharacters(
								idFilter
							)}', Edm.String)`,
					],
				},
				orderBy: `${convertProjectOrderByToQueryParam(
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
				.get<ByPageResponse<ProjectEntity>>(
					`${GET_PROJECTS_API_ENDPOINT}/search/${query}`
				)
				.then((res) => res.data);
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
};
