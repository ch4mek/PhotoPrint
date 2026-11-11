import { useMutation, MutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { NotificationType, PROJECT_ADDED } from '@/constants/notifications';
import { GET_PROJECTS_API_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';

import { CreateProjectOptions } from './types';

export const useCreateProject = (
	options: MutationOptions<
		CreateProjectOptions,
		AxiosError,
		CreateProjectOptions
	>,
	toast: ToastContextValues
) =>
	useMutation<CreateProjectOptions, AxiosError, CreateProjectOptions>({
		...options,
		mutationFn: ({ client, name, isPublic, manager, ...otherValues }) =>
			axios
				.post(GET_PROJECTS_API_ENDPOINT, {
					...otherValues,
					clientId: client.id,
					managerId: manager?.id,
					name,
					isPublic,
				})
				.then((res) => {
					toast.open(createToast(NotificationType.SUCCESS, PROJECT_ADDED));

					return res.data;
				})
				.catch((error) => {
					if (error?.response?.data?.Message) {
						toast.open(
							createToast(NotificationType.ERROR, error.response.data.Message)
						);
					}
				}),
	});
