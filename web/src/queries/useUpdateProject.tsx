import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { NotificationType, PROJECT_UPDATED } from '@/constants/notifications';
import { GET_PROJECTS_API_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';
import { ProjectEntity, UseUpdateProjectParams } from '@/types/project';

export const useUpdateProject = (
	options: UseMutationOptions<
		ProjectEntity,
		AxiosError,
		UseUpdateProjectParams
	>,
	toast: ToastContextValues
) =>
	useMutation<ProjectEntity, AxiosError, UseUpdateProjectParams>({
		...options,
		mutationFn: ({ client, name, isPublic, manager, ...otherValues }) =>
			axios
				.put(GET_PROJECTS_API_ENDPOINT, {
					...otherValues,
					clientId: client.id,
					managerId: manager?.id,
					name,
					isPublic,
				})
				.then((res) => {
					toast.open(createToast(NotificationType.SUCCESS, PROJECT_UPDATED));

					return res.data;
				})
				.catch((error) => {
					if (error?.response?.data?.Message) {
						toast.open(
							createToast(
								NotificationType.ERROR,
								error?.response?.data?.Message
							)
						);
					}
				}),
	});
