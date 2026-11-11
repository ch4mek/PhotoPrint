import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';
import buildQuery from 'odata-query';

import { ActivityStatus } from '@/constants/activityStatus';
import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { UsersQueriesKeys } from '@/constants/queriesKeys';
import { GET_USERS_TAB_SEARCH_API_ENDPOINT } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { convertOrderToQueryParam } from '@/helpers/convertOrderToQueryParam';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { UsersFilterType } from '@/pages/AdminPanel/components/UsersTab/types';
import { getOrderByParams } from '@/pages/AdminPanel/components/UsersTab/utils';
import { ByPageResponse } from '@/queries/types';
import { DepartmentType, LocationType, RoleType } from '@/types/configuration';
import { UseUsersResponse } from '@/types/userInfo';

import { convertFilterArrayToQueryParam } from './utils';
import { getLogicFilterValue } from './utils/getLogicFilterValue';

const queryParamsToReverseOrder = ['positionOrderWeight', 'isActive'];

const convertUserOrderByQueryParam = (orderBy: string) =>
	orderBy !== 'position' ? orderBy : 'positionOrderWeight';

export const useUsersTab = (
	page: number,
	rowsPerPage: number,
	order: SortDirection,
	orderByCondition: string,
	filters: UsersFilterType,
	allDepartaments: Array<DepartmentType>,
	allLocations: Array<LocationType>,
	allRoles: Array<RoleType>
) =>
	useQueryWithCallback<ByPageResponse<UseUsersResponse>>({
		queryKey: [
			UsersQueriesKeys.usersTab,
			page,
			rowsPerPage,
			order,
			orderByCondition,
			filters,
			allDepartaments,
			allLocations,
			allRoles,
		],
		queryFn: () => {
			const {
				email,
				id,
				user,
				departments,
				position,
				location,
				status,
				group,
			} = filters;

			const convertedOrderBy = convertUserOrderByQueryParam(orderByCondition);
			const convertedOrder = convertOrderToQueryParam(
				order,
				convertedOrderBy,
				queryParamsToReverseOrder
			);
			const orderBy = getOrderByParams(convertedOrderBy, convertedOrder);

			const query = buildQuery({
				filter: {
					and: [
						user
							? `contains(tolower(combinedFullName), tolower('${replaceSpecialCharacters(
									user
							  )}'))`
							: undefined,
						email
							? `contains(tolower(email), tolower('${replaceSpecialCharacters(
									email
							  )}'))`
							: undefined,
						id &&
							`cast(id, Edm.String) eq cast('${replaceSpecialCharacters(
								id
							)}', Edm.String)`,
						group
							? `contains(tolower(resourceGroupName), tolower('${replaceSpecialCharacters(
									group
							  )}'))`
							: undefined,
						status
							? getLogicFilterValue(status, ActivityStatus.Active, 'isActive')
							: undefined,
						{
							departmentId: convertFilterArrayToQueryParam(
								departments,
								[...allDepartaments, NO_VALUE_FILTER_OPTION].length
							),
						},
						{
							position: convertFilterArrayToQueryParam(
								position,
								allRoles.length
							),
						},
						{
							location: convertFilterArrayToQueryParam(
								location,
								allLocations.length
							),
						},
					],
				},
				orderBy,
				top: rowsPerPage,
				skip: (page - 1) * rowsPerPage,
			});

			return axios
				.get<ByPageResponse<UseUsersResponse>>(
					`${GET_USERS_TAB_SEARCH_API_ENDPOINT}${query}`
				)
				.then((res) => res.data);
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
