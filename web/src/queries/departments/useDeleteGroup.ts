import { useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { GROUP_DELETED, NotificationType } from '@/constants/notifications';
import { departmentGroupEndpoint } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';
import {
	createOnErrorHandler,
	handleRequestError,
} from '@/helpers/requestErrorHandler';

export const useDeleteGroup = (toast: ToastContextValues) =>
	useMutation<boolean, AxiosError<ServerResponseError>, number>({
		mutationFn: async (id) => {
			try {
				const { data } = await axios.delete(departmentGroupEndpoint(id));

				return data;
			} catch (err) {
				return handleRequestError(err);
			}
		},
		onSuccess: () => {
			toast.open(createToast(NotificationType.SUCCESS, GROUP_DELETED));
		},
		onError: createOnErrorHandler(toast),
	});
