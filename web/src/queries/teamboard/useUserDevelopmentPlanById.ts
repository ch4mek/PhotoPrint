import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { getDevelopmentPlanEndpointUrl } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { UserDevelopmentPlan } from '@/types/developmentPlan';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useUserDevelopmentPlanById = (
	developmentPlanId: number | undefined,
	options?: PatchedQueryOptions<
		UserDevelopmentPlan,
		AxiosError<ServerResponseError>,
		UserDevelopmentPlan
	>
) =>
	useQueryWithCallback({
		queryKey: [TeamboardQueriesKeys.getUserConversation, developmentPlanId],
		queryFn: async () => {
			try {
				const { data } = await axios.get<UserDevelopmentPlan>(
					`${getDevelopmentPlanEndpointUrl(developmentPlanId!)}`
				);

				return data;
			} catch (error) {
				throw error as AxiosError<ServerResponseError>;
			}
		},

		refetchOnWindowFocus: false,
		...options,
	});
