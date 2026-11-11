import { UseQueryOptions, keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_PLAN_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PlanResponse } from '@/types/plan';

export const useGetPlan = (
	id: number,
	queryOptions?: UseQueryOptions<PlanResponse, AxiosError<ServerResponseError>>
) =>
	useQueryWithCallback<PlanResponse, AxiosError<ServerResponseError>>({
		queryKey: [TeamboardQueriesKeys.plan, id],
		queryFn: async () => {
			const { data } = await axios.get<PlanResponse>(
				`${GET_USER_PLAN_API_ENDPOINT}`,
				{
					params: {
						userId: id,
					},
				}
			);

			return data;
		},
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
		...queryOptions,
	});
