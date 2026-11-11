import {
	useMutation,
	UseMutationOptions,
	useQueryClient,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ratesQueriesKeys } from '@/constants/queriesKeys';
import { getProjectRateByIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { ProjectRateEntity, UpdateProjectRateParams } from '@/types/rates';

export const useUpdateProjectRate = (
	options?: UseMutationOptions<
		ProjectRateEntity,
		AxiosError<ServerResponseError>,
		UpdateProjectRateParams
	>
) => {
	const queryClient = useQueryClient();
	const { onSuccess, ...restOptions } = options ?? {};

	return useMutation({
		...restOptions,
		mutationFn: async ({ id, ...params }) => {
			try {
				const response = await axios.patch<ProjectRateEntity>(
					getProjectRateByIdEndpoint(id),
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
