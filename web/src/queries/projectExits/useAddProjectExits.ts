import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError, AxiosResponse } from 'axios';

import { ServerResponseError } from '../types';

import { GET_EXITS_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { AddProjectExitPayload, ProjectExitEntity } from '@/types/projectExits';

export const useAddProjectExits = (
	options?: UseMutationOptions<
		AxiosResponse<ProjectExitEntity>,
		AxiosError<ServerResponseError>,
		AddProjectExitPayload
	>
) =>
	useMutation({
		mutationFn: async (data) => {
			try {
				const response = await axios.post(GET_EXITS_API_ENDPOINT, data);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
