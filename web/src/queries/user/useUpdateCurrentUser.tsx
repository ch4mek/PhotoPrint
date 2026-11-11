import { UseMutationOptions, useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { NotificationType, PROFILE_UPDATING } from '@/constants/notifications';
import { UPDATE_CURRENT_USER } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';
import { UserInfo } from '@/types/userInfo';

import { RequestData } from './types';

export const useUpdateCurrentUser = (
	options: UseMutationOptions<
		UserInfo,
		AxiosError<ServerResponseError, unknown>,
		RequestData,
		unknown
	>,
	toast: ToastContextValues
) =>
	useMutation<UserInfo, AxiosError<ServerResponseError, unknown>, RequestData>({
		...options,
		mutationFn: ({
			id,
			firstName,
			lastName,
			departmentId,
			location,
			timeZone,
			avatarFileId,
			message,
		}: RequestData) =>
			axios
				.put<UserInfo>(`${UPDATE_CURRENT_USER}`, {
					id,
					firstName,
					lastName,
					departmentId,
					location,
					timeZone,
					avatarFileId,
				})
				.then((res) => {
					toast.open(
						createToast(NotificationType.SUCCESS, message ?? PROFILE_UPDATING)
					);

					return res.data;
				})
				.catch((error) => Promise.reject(error)),
	});
