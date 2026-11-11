import { UseMutationOptions, useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { NotificationType, CLIENT_ADDED } from '@/constants/notifications';
import { GET_CLIENTS_API_ENDPOINT } from '@/constants/queryPaths';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';

import { CreateClientOptions, CreateClientsResponse } from './types';

export const useCreateClient = (
	options: UseMutationOptions<
		CreateClientsResponse,
		AxiosError,
		CreateClientOptions
	>
) => {
	const toast = useToast();

	return useMutation<CreateClientsResponse, AxiosError, CreateClientOptions>({
		...options,
		mutationFn: ({
			name,
			legalEntityName,
			comments,
			registeredLocation,
			color,
			salesManagerId,
		}) =>
			axios
				.post(GET_CLIENTS_API_ENDPOINT, {
					name,
					legalEntityName: legalEntityName || null,
					comments: comments || null,
					registeredLocation,
					salesManagerId,
					color,
				})
				.then((res) => {
					toast.open(createToast(NotificationType.SUCCESS, CLIENT_ADDED));

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
};
