import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_SPECIALIZATION_API_ENDPOINT } from '@/constants/queryPaths';
import { ServerResponseError } from '@/queries/types';
import { DetailedSpecialization } from '@/types/settings';

export const useGetSpecializationById = (
	options: UseMutationOptions<
		DetailedSpecialization,
		AxiosError<ServerResponseError>,
		string | number
	>
) =>
	useMutation({
		mutationFn: async (id: number | string) => {
			try {
				const { data } = await axios.get<DetailedSpecialization>(
					`${GET_SPECIALIZATION_API_ENDPOINT}/${id}`
				);

				return data;
			} catch (error) {
				return Promise.reject(error);
			}
		},
		...options,
	});
