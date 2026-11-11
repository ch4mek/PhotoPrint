import { UseMutationOptions, useMutation } from '@tanstack/react-query';
import axios, { AxiosError, HttpStatusCode } from 'axios';

import { useToast } from '../context';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import {
	CANNOT_DEACTIVATE_USER,
	NotificationType,
	USER_NOT_FOUND,
} from '@/constants/notifications';
import { GET_USERS_API_ENDPOINT } from '@/constants/queryPaths';
import { createToast } from '@/helpers/createToast';

import { ServerResponseError, UpdateUserStatusParams } from './types';

export const useUpdateUserStatus = ({
	onSuccess,
	onError,
	...options
}: UseMutationOptions<
	unknown,
	AxiosError<ServerResponseError>,
	UpdateUserStatusParams
>) => {
	const toast = useToast();

	return useMutation<
		unknown,
		AxiosError<ServerResponseError>,
		UpdateUserStatusParams
	>({
		...options,
		mutationFn: ({ id, isActive }) =>
			axios.put(`${GET_USERS_API_ENDPOINT}/${id}/IsActive`, {
				isActive,
			}),
		onSuccess(data, variables, context) {
			const { isActive } = variables;
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					`User is successfully ${isActive ? 'activated' : 'deactivated'}`
				)
			);

			if (onSuccess) {
				onSuccess(data, variables, context);
			}
		},

		onError(error, variables, context) {
			const { isActive } = variables;

			if (!isActive) {
				switch (error?.response?.status) {
					case HttpStatusCode.Forbidden:
						toast.open(
							createToast(NotificationType.ERROR, CANNOT_DEACTIVATE_USER)
						);
						break;
					case HttpStatusCode.NotFound:
						toast.open(createToast(NotificationType.ERROR, USER_NOT_FOUND));
						break;

					default:
						toast.open(
							createToast(
								NotificationType.ERROR,
								error.response?.data?.Message ?? SOMETHING_WENT_WRONG
							)
						);
						break;
				}
			}

			if (isActive) {
				switch (error?.response?.status) {
					case HttpStatusCode.Forbidden:
						toast.open(
							createToast(NotificationType.ERROR, error.response?.data?.Message)
						);
						break;
					case HttpStatusCode.NotFound:
						toast.open(createToast(NotificationType.ERROR, USER_NOT_FOUND));
						break;

					default:
						toast.open(
							createToast(
								NotificationType.ERROR,
								error.response?.data?.Message ?? SOMETHING_WENT_WRONG
							)
						);
						break;
				}
			}

			if (onError) {
				onError(error, variables, context);
			}
		},
	});
};
