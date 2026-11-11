import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import {
	NOTIFICATION_MESSAGE,
	NotificationType,
} from '@/constants/notifications';
import { GET_PROJECTS_API_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';
import { UpdateProjectStatusParams } from '@/queries/types';
import { ProjectEntity } from '@/types/project';

export const useUpdateProjectStatus = (
	options: UseMutationOptions<
		ProjectEntity,
		AxiosError,
		UpdateProjectStatusParams
	>,
	toast: ToastContextValues
) =>
	useMutation<ProjectEntity, AxiosError, UpdateProjectStatusParams>({
		...options,
		mutationFn: ({ id, isActive }) =>
			axios
				.put(`${GET_PROJECTS_API_ENDPOINT}/${id}/IsActive`, {
					isActive,
				})
				.then((res) => {
					toast.open(
						createToast(
							NotificationType.SUCCESS,
							`Project is successfully ${
								isActive ? 'activated' : 'deactivated'
							}`
						)
					);

					return res.data;
				})
				.catch((error) => {
					if (error?.response?.data?.Message) {
						toast.open(
							createToast(
								NotificationType.ERROR,
								error.response.data.Message.includes(
									'There are users on the project'
								)
									? NOTIFICATION_MESSAGE.CANNOT_DEACTIVATE_PROJECT
									: error.response.data.Message || SOMETHING_WENT_WRONG
							)
						);
					}

					throw error;
				}),
	});
