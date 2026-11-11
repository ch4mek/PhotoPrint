import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_SALARY_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { UserSalary } from '@/types/teamboard';

export const useUserSalary = (
	id: number,
	queryOptions?: PatchedQueryOptions<
		UserSalary,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<UserSalary, AxiosError<ServerResponseError>>({
		queryKey: [TeamboardQueriesKeys.userHardSkills, id],
		queryFn: async () => {
			const { data } = await axios.get<UserSalary>(
				`${GET_USER_SALARY_ENDPOINT}/${id}`
			);

			return data;
		},
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
		...queryOptions,
	});
