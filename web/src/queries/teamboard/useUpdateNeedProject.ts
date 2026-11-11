import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { NeedProjectSubmitParams, ServerResponseError } from '../types';

import { GET_USER_NEED_PROJECT_API_ENDPOINT } from '@/constants/queryPaths';
import { PersonalDataResponse } from '@/types/personalData';

export const useUpdateNeedProject = (
	options: UseMutationOptions<
		PersonalDataResponse,
		AxiosError<ServerResponseError>,
		NeedProjectSubmitParams
	>
) =>
	useMutation<
		PersonalDataResponse,
		AxiosError<ServerResponseError>,
		NeedProjectSubmitParams
	>({
		...options,
		mutationFn: ({ userId, needProject }) =>
			axios
				.patch(
					GET_USER_NEED_PROJECT_API_ENDPOINT,
					{},
					{
						params: {
							userId,
							isUserNeedProject: needProject,
						},
					}
				)
				.then((res) => res.data)
				.catch((error) => {
					throw error;
				}),
	});
