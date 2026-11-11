import { UseMutationOptions, useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { GET_PERMISSIONS_ASSIGNMENTS_API_ENDPOINT } from '@/constants/queryPaths';
import { useToast } from '@/context/index';
import { createToast } from '@/helpers/createToast';
import { ServerResponseError } from '@/queries/types';

import { RevokePermissionAssignmentParams } from './types';

export const useRevokePermissionAssignment = (
	options: UseMutationOptions<
		void,
		AxiosError<ServerResponseError>,
		RevokePermissionAssignmentParams
	>
) => {
	const toast = useToast();

	return useMutation<
		void,
		AxiosError<ServerResponseError>,
		RevokePermissionAssignmentParams
	>({
		...options,
		mutationFn: async ({ assignmentId }: RevokePermissionAssignmentParams) => {
			await axios.delete(
				`${GET_PERMISSIONS_ASSIGNMENTS_API_ENDPOINT}/${assignmentId}`
			);
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
