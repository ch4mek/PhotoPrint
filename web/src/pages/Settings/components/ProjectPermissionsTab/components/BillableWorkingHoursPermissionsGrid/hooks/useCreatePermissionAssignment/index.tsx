import { UseMutationOptions, useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { GET_PERMISSIONS_ASSIGNMENTS_API_ENDPOINT } from '@/constants/queryPaths';
import { useToast } from '@/context/index';
import { createToast } from '@/helpers/createToast';
import { ServerResponseError } from '@/queries/types';

import { CreatePermissionAssignmentOptions } from './types';

export const useCreatePermissionAssignment = (
	options: UseMutationOptions<
		void,
		AxiosError<ServerResponseError>,
		CreatePermissionAssignmentOptions
	>
) => {
	const toast = useToast();

	return useMutation<
		void,
		AxiosError<ServerResponseError>,
		CreatePermissionAssignmentOptions
	>({
		...options,
		mutationFn: async ({ userId, permissionType, resources, resourceType }) => {
			await axios.put(GET_PERMISSIONS_ASSIGNMENTS_API_ENDPOINT, {
				userId,
				permissionType,
				resources,
				resourceType,
			});
		},
		onSuccess(data, variables, context) {
			if (options.onSuccess) {
				options.onSuccess(data, variables, context);
			}
		},
		onError(error, variables, context) {
			toast.open(
				createToast(
					NotificationType.ERROR,
					error?.response?.data?.Message ?? SOMETHING_WENT_WRONG
				)
			);

			if (options.onError) {
				options.onError(error, variables, context);
			}
		},
	});
};
