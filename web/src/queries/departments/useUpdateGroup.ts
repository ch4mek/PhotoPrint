import { useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { GROUP_UPDATED, NotificationType } from '@/constants/notifications';
import { departmentGroupEndpoint } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';
import {
	createOnErrorHandler,
	handleRequestError,
} from '@/helpers/requestErrorHandler';
import { Group, UpdateGroupRequestBody } from '@/types/department';

export const useUpdateGroup = (toast: ToastContextValues) =>
	useMutation<Group, AxiosError<ServerResponseError>, UpdateGroupRequestBody>({
		mutationFn: async (body) => {
			try {
				const { id } = body;

				const { data } = await axios.put(departmentGroupEndpoint(id), body);

				return data;
			} catch (err) {
				return handleRequestError(err);
			}
		},
		onSuccess: () => {
			toast.open(createToast(NotificationType.SUCCESS, GROUP_UPDATED));
		},
		onError: createOnErrorHandler(toast),
	});
