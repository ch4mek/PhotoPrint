import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { GET_SPECIALIZATION_API_ENDPOINT } from '@/constants/queryPaths';
import { Specialization } from '@/types/settings';

export const useDeleteSpecialization = (
	options: UseMutationOptions<
		Specialization,
		AxiosError<ServerResponseError>,
		Specialization
	>
) =>
	useMutation({
		mutationFn: async ({ id }: Specialization) => {
			try {
				const { data } = await axios.delete(
					`${GET_SPECIALIZATION_API_ENDPOINT}/${id}`
				);

				return data;
			} catch (error) {
				return Promise.reject(error);
			}
		},
		...options,
	});
