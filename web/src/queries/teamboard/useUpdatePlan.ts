import {
	useMutation,
	useQueryClient,
	UseMutationOptions,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import {
	TeamboardQueriesKeys,
	payloadQueriesKeys,
} from '@/constants/queriesKeys';
import { GET_USER_PLAN_API_ENDPOINT } from '@/constants/queryPaths';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { PlanResponse, UpdatePlan } from '@/types/plan';

export const useUpdatePlan = (
	options: UseMutationOptions<PlanResponse, AxiosError, UpdatePlan>
) => {
	const queryClient = useQueryClient();

	return useMutation<PlanResponse, AxiosError, UpdatePlan>({
		...options,
		onSuccess: (...args) => {
			const [, { userId }] = args;

			queryClient.invalidateQueries({
				queryKey: [payloadQueriesKeys.getPayloadData],
			});
			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.plan, userId],
			});

			return options.onSuccess?.(...args);
		},
		mutationFn: ({ plan, userId }) =>
			axios
				.patch(
					GET_USER_PLAN_API_ENDPOINT,
					{
						plan: !plan ? null : plan,
					},
					{
						params: {
							userId,
						},
					}
				)
				.then((res) => res.data)
				.catch((error) => {
					createOnErrorHandler(error);
				}),
	});
};
