import { useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { GROUP_CREATED, NotificationType } from '@/constants/notifications';
import { createGroupEndpoint } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';
import {
	createOnErrorHandler,
	handleRequestError,
} from '@/helpers/requestErrorHandler';
import { CreateGroupRequestBody } from '@/types/department';

import { CreateGroupResponse } from './types';

export const useCreateGroup = (toast: ToastContextValues) =>
	useMutation<
		CreateGroupResponse,
		AxiosError<ServerResponseError>,
		CreateGroupRequestBody
	>({
		mutationFn: async (body) => {
			try {
				const { departmentId } = body;

				const { data } = await axios.post(
					createGroupEndpoint(departmentId),
					body
				);

				return data;
			} catch (err) {
				return handleRequestError(err);
			}
		},
		onSuccess: () => {
			toast.open(createToast(NotificationType.SUCCESS, GROUP_CREATED));
		},
		onError: createOnErrorHandler(toast),
	});
