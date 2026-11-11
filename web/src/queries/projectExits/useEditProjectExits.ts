import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError, AxiosResponse } from 'axios';

import { ServerResponseError } from '../types';

import { getProjectExitByIdApiEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import {
	EditProjectExitPayload,
	ProjectExitEntity,
} from '@/types/projectExits';

export const useEditProjectExits = (
	options?: UseMutationOptions<
		AxiosResponse<ProjectExitEntity>,
		AxiosError<ServerResponseError>,
		EditProjectExitPayload
	>
) =>
	useMutation({
		mutationFn: async ({ projectExitId, ...data }) => {
			try {
				const response = await axios.put(
					getProjectExitByIdApiEndpoint(projectExitId),
					data
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
