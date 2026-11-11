import axios, { AxiosError } from 'axios';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { getUserSpecializationsEndpointUrl } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { UserNotAssignedSpecializationsResponse } from './types';

export const useUserSpecializations = (
	userId: number | undefined,
	areAssigned: boolean,
	options?: PatchedQueryOptions<
		UserNotAssignedSpecializationsResponse,
		AxiosError
	>
) =>
	useQueryWithCallback<UserNotAssignedSpecializationsResponse, AxiosError>({
		queryKey: [
			TeamboardQueriesKeys.userNotAssignedSpecializations,
			userId,
			areAssigned,
		],
		queryFn: async () => {
			const { data } = await axios.get<UserNotAssignedSpecializationsResponse>(
				getUserSpecializationsEndpointUrl(userId, areAssigned)
			);

			return data;
		},
		refetchOnWindowFocus: false,
		enabled: Boolean(userId),
		...options,
	});
