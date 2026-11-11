import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { SettingsUsersClaimsQueriesKeys } from '@/constants/queriesKeys';
import { GET_SETTINGS_USERS_CLAIMS_ENDPOINT } from '@/constants/queryPaths';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

import {
	GetAdminUsersClaimsRequestParams,
	GetAdminUsersClaimsResponse,
	ServerResponseError,
} from './types';
import { convertFilterArrayToQueryParam } from './utils';

export const useGetAllUsersClaims = ({
	page,
	rowsPerPage,
	order,
	orderByCondition,
	filters,
	queryOptions,
	allDepartmentsLength,
	claimName,
}: GetAdminUsersClaimsRequestParams) =>
	useQueryWithCallback<
		GetAdminUsersClaimsResponse,
		AxiosError<ServerResponseError>
	>({
		queryKey: [
			SettingsUsersClaimsQueriesKeys.usersClaims,
			page,
			rowsPerPage,
			order,
			orderByCondition,
			filters,
			claimName,
		],
		queryFn: async () => {
			const { userClaimName, userId, departments } = filters || {};
			const query = buildQuery({
				filter: {
					and: [
						userClaimName &&
							`(contains(tolower(userFullName), tolower('${replaceSpecialCharacters(
								userClaimName
							)}')))`,
						userId &&
							`cast(userId, Edm.String) eq cast('${replaceSpecialCharacters(
								userId
							)}', Edm.String)`,
						departments && {
							departmentId: convertFilterArrayToQueryParam(
								departments,
								allDepartmentsLength
							),
						},
					],
				},
				top: rowsPerPage,
				skip: page * rowsPerPage,
				orderBy:
					orderByCondition && order ? `${orderByCondition} ${order}` : '',
			});

			const res = await axios.get<GetAdminUsersClaimsResponse>(
				`${GET_SETTINGS_USERS_CLAIMS_ENDPOINT}${query}`,
				{ params: { claimName } }
			);

			return res.data;
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		...queryOptions,
	});
