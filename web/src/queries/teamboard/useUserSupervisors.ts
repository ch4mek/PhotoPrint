import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_SUPERVISORS_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { UserSupervisor } from '@/types/teamboard';

export const useUserSupervisors = (
	userId: number,
	queryOptions?: PatchedQueryOptions<Array<UserSupervisor>, AxiosError>
) =>
	useQueryWithCallback<Array<UserSupervisor>, AxiosError>({
		...queryOptions,
		queryKey: [TeamboardQueriesKeys.userHardSkills, userId],
		queryFn: async () => {
			const { data } = await axios.get<Array<UserSupervisor>>(
				`${GET_USER_SUPERVISORS_ENDPOINT}`,
				{
					params: {
						userId,
					},
				}
			);

			return data;
		},
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
		...queryOptions,
	});
