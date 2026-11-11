import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { NotificationType } from '@/constants/notifications';
import { GET_USER_PROJECTS_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';

import {
	EditUserProjectAssignParams,
	EditUserProjectAssignResponse,
	ServerResponseError,
} from './types';
import { convertProjectMutationErrorToToast } from './utils/convertProjectMutationErrorToToast';

export const useEditUserProjectAssign = (
	toast: ToastContextValues,
	options?: UseMutationOptions<
		EditUserProjectAssignResponse,
		AxiosError<ServerResponseError>,
		EditUserProjectAssignParams
	>
) =>
	useMutation<
		EditUserProjectAssignResponse,
		AxiosError<ServerResponseError>,
		EditUserProjectAssignParams
	>({
		mutationFn: async (params: EditUserProjectAssignParams) => {
			const { data } = await axios.put<EditUserProjectAssignResponse>(
				GET_USER_PROJECTS_ENDPOINT,
				params
			);

			return data;
		},
		...options,
		onError: (error) => {
			if (error.response?.data.Message) {
				const errorMessage = convertProjectMutationErrorToToast(
					error.response?.data.Message
				);
				toast.open(createToast(NotificationType.ERROR, errorMessage));
			}
		},
	});
