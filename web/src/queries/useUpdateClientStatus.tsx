import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_CLIENTS_API_ENDPOINT } from '@/constants/queryPaths';
import { UpdateClientStatusParams } from '@/queries/types';
import { ClientUpdateStatusResponse } from '@/types/clients';

export const useUpdateClientStatus = (
	options: UseMutationOptions<
		ClientUpdateStatusResponse,
		AxiosError,
		UpdateClientStatusParams
	>
) =>
	useMutation<ClientUpdateStatusResponse, AxiosError, UpdateClientStatusParams>(
		{
			...options,
			mutationFn: ({ id, newStatus }) =>
				axios
					.patch(`${GET_CLIENTS_API_ENDPOINT}/${id}/isActive`, {
						isActive: newStatus,
					})
					.then((res) => res.data)
					.catch((error) => {
						throw error;
					}),
		}
	);
