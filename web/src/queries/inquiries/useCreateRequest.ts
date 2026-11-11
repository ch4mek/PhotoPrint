import {
	useMutation,
	UseMutationOptions,
	useQueryClient,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { absencesQueriesKeys, homeQueriesKeys } from '@/constants/queriesKeys';
import { getUserInquiries } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';

import { Inquiry, RequestAddData } from './types';

export const useCreateRequest = (
	options?: UseMutationOptions<
		Inquiry,
		AxiosError<ServerResponseError>,
		RequestAddData
	>
) => {
	const queryClient = useQueryClient();
	const { onSuccess, ...restOptions } = options ?? {};

	return useMutation({
		mutationFn: async ({ userId, ...data }) => {
			try {
				const response = await axios.post(getUserInquiries(userId), data);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		onSuccess: (data, variables, context) => {
			queryClient.invalidateQueries({
				queryKey: [absencesQueriesKeys.getRequestsAbsences],
			});

			queryClient.invalidateQueries({
				queryKey: [absencesQueriesKeys.getUserAbsences],
			});

			queryClient.invalidateQueries({
				queryKey: [homeQueriesKeys.getUserUpcomingEvents],
			});

			queryClient.invalidateQueries({
				queryKey: [absencesQueriesKeys.getInquiriesCalendar],
			});

			onSuccess?.(data, variables, context);
		},
		...restOptions,
	});
};
