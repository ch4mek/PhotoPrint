import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { GET_SPECIALIZATION_API_ENDPOINT } from '@/constants/queryPaths';
import { EditSpecializationParams, Specialization } from '@/types/settings';

export const useUpdateSpecialization = (
	options: UseMutationOptions<
		Specialization,
		AxiosError<ServerResponseError>,
		EditSpecializationParams
	>
) =>
	useMutation<
		Specialization,
		AxiosError<ServerResponseError>,
		EditSpecializationParams
	>({
		...options,
		mutationFn: ({ name, id, departments, hardSkills }) =>
			axios
				.put(GET_SPECIALIZATION_API_ENDPOINT, {
					id,
					name,
					hardSkills,
					departmentIds: departments.map((department) => department.id),
				})
				.then((res) => res.data)
				.catch((error) => {
					throw error;
				}),
	});
