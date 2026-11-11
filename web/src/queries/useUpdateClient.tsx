import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { CLIENT_UPDATED, NotificationType } from '@/constants/notifications';
import { GET_CLIENTS_API_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';
import { UpdateClientsParams, UpdateClientsResponse } from '@/queries/types';

export const useUpdateClient = (
	options: UseMutationOptions<
		UpdateClientsResponse,
		AxiosError,
		UpdateClientsParams
	>,
	toast: ToastContextValues
) =>
	useMutation<UpdateClientsResponse, AxiosError, UpdateClientsParams>({
		...options,
		mutationFn: ({
			comments,
			name,
			legalEntityName,
			registeredLocation,
			id,
			color,
			salesManagerId,
			isDeleted,
		}) =>
			axios
				.put(GET_CLIENTS_API_ENDPOINT, {
					comments: comments || null,
					name,
					legalEntityName: legalEntityName || null,
					registeredLocation,
					id,
					color,
					salesManagerId,
					isDeleted,
				})
				.then((res) => {
					toast.open(createToast(NotificationType.SUCCESS, CLIENT_UPDATED));

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
