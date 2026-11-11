import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_INTERVIEWS_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { CreateUpdateInterviewTemplate } from '@/types/interview';

export const useAddInterview = (
	options?: UseMutationOptions<
		CreateUpdateInterviewTemplate,
		AxiosError<ServerResponseError>,
		CreateUpdateInterviewTemplate
	>
) =>
	useMutation({
		mutationFn: async (data) => {
			try {
				const response = await axios.post(GET_INTERVIEWS_ENDPOINT, data);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
