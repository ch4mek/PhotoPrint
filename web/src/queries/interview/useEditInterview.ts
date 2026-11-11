import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getInterviewByIdApiEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { UpdateInterview } from '@/types/interview';

export const useEditInterview = (
	options?: UseMutationOptions<
		UpdateInterview,
		AxiosError<ServerResponseError>,
		UpdateInterview
	>
) =>
	useMutation({
		mutationFn: async ({ interviewId, ...data }) => {
			try {
				const response = await axios.put(
					getInterviewByIdApiEndpoint(interviewId),
					data
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
