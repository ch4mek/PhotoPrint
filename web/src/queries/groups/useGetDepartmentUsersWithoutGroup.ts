import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ByPageResponse, ServerResponseError } from '../types';

import { GroupQueriesKeys } from '@/constants/queriesKeys';
import { GET_USERS_API_ENDPOINT } from '@/constants/queryPaths';
import { Roles } from '@/constants/roles';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { UseUsersResponse } from '@/types/userInfo';

import { UseGetDepartmentUsersWithoutGroupProps } from './types';

const TOP_LIMIT = 10000;

export const useGetDepartmentUsersWithoutGroup = ({
	departmentId,
	options,
}: UseGetDepartmentUsersWithoutGroupProps) =>
	useQueryWithCallback<
		Array<UseUsersResponse>,
		AxiosError<ServerResponseError>
	>({
		...options,
		queryKey: [GroupQueriesKeys.departmentUsersWithoutGroup, departmentId],
		queryFn: async () => {
			const query = buildQuery({
				filter: {
					and: [
						{
							departmentId,
						},
						{
							resourceGroupName: null,
						},
						{
							isActive: true,
						},
						{
							position: Roles.SE,
						},
					],
				},
				top: TOP_LIMIT,
				skip: 0,
			});

			const { data } = await axios.get<ByPageResponse<UseUsersResponse>>(
				`${GET_USERS_API_ENDPOINT}/search${query}`
			);

			return data.items;
		},
	});
