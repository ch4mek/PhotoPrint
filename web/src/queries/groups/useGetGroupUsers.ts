import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ByPageResponse, ServerResponseError } from '../types';
import { convertFilterArrayToQueryParam } from '../utils';

import { GroupQueriesKeys } from '@/constants/queriesKeys';
import { departmentGroupUsersEndpoint } from '@/constants/queryPaths';
import { convertOrderToQueryParam } from '@/helpers/convertOrderToQueryParam';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

import { GroupUsersContract, UseGetGroupUsersProps } from './types';

const queryParamsToReverseOrder = ['isActive'];

export const useGetGroupUsers = ({
	groupId,
	options,
	rowsPerPage,
	page,
	filters,
	allLocations,
	order,
	orderBy,
}: UseGetGroupUsersProps) =>
	useQueryWithCallback<
		ByPageResponse<GroupUsersContract>,
		AxiosError<ServerResponseError>
	>({
		...options,
		queryKey: [
			GroupQueriesKeys.groupMembers,
			groupId,
			page,
			order,
			orderBy,
			JSON.stringify(filters),
			rowsPerPage,
		],
		queryFn: async () => {
			const { userId, userName, email, location } = filters;

			const convertedOrder = convertOrderToQueryParam(
				order,
				orderBy,
				queryParamsToReverseOrder
			);

			const query = buildQuery({
				filter: {
					and: [
						userName
							? `contains(tolower(fullName), tolower('${replaceSpecialCharacters(
									userName
							  )}'))`
							: undefined,
						email
							? `contains(tolower(email), tolower('${replaceSpecialCharacters(
									email
							  )}'))`
							: undefined,
						userId &&
							`cast(id, Edm.String) eq cast('${replaceSpecialCharacters(
								userId
							)}', Edm.String)`,
						{
							location: convertFilterArrayToQueryParam(
								location,
								allLocations.length
							),
						},
					],
				},
				orderBy: `${orderBy} ${convertedOrder}`,
				top: rowsPerPage,
				skip: page * rowsPerPage,
			});

			const { data } = await axios.get(
				`${departmentGroupUsersEndpoint(groupId)}${query}`
			);

			return data;
		},
		refetchOnWindowFocus: false,
	});
