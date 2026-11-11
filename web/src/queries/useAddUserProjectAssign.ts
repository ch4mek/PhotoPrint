import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { NotificationType } from '@/constants/notifications';
import { createUserProjectAssignmentEndpointUrl } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';

import {
	AddUserProjectAssignParams,
	AddUserProjectAssignResponse,
	ServerResponseError,
} from './types';
import { convertProjectMutationErrorToToast } from './utils/convertProjectMutationErrorToToast';

export const useAddUserProjectAssign = (
	toast: ToastContextValues,
	options?: UseMutationOptions<
		AddUserProjectAssignResponse,
		AxiosError<ServerResponseError>,
		AddUserProjectAssignParams
	>
) =>
	useMutation<
		AddUserProjectAssignResponse,
		AxiosError<ServerResponseError>,
		AddUserProjectAssignParams
	>({
		mutationFn: async ({
			userId,
			projectId,
			...restParams
		}: AddUserProjectAssignParams) => {
			const { data } = await axios.post<AddUserProjectAssignResponse>(
				createUserProjectAssignmentEndpointUrl(userId, projectId),
				restParams
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
