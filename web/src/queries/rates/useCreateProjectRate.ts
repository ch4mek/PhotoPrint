import {
	useMutation,
	UseMutationOptions,
	useQueryClient,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ratesQueriesKeys } from '@/constants/queriesKeys';
import { GET_PROJECT_RATES_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { CreateProjectRateParams, ProjectRateEntity } from '@/types/rates';

export const useCreateProjectRate = (
	options?: UseMutationOptions<
		ProjectRateEntity,
		AxiosError<ServerResponseError>,
		CreateProjectRateParams
	>
) => {
	const queryClient = useQueryClient();
	const { onSuccess, ...restOptions } = options ?? {};

	return useMutation({
		...restOptions,
		mutationFn: async (params) => {
			try {
				const response = await axios.post<ProjectRateEntity>(
					GET_PROJECT_RATES_API_ENDPOINT,
					params
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		onSuccess: (data, variables, context) => {
			queryClient.invalidateQueries({
				queryKey: [ratesQueriesKeys.getRateHistory],
			});
			queryClient.invalidateQueries({
				queryKey: [ratesQueriesKeys.getRates],
			});

			onSuccess?.(data, variables, context);
		},
	});
};
