import { useMutation, MutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { GET_SPECIALIZATION_API_ENDPOINT } from '@/constants/queryPaths';
import { CreateSpecializationParams, Specialization } from '@/types/settings';

export const useCreateSpecialization = (
	options: MutationOptions<
		Specialization,
		AxiosError<ServerResponseError>,
		CreateSpecializationParams
	>
) =>
	useMutation<
		Specialization,
		AxiosError<ServerResponseError>,
		CreateSpecializationParams
	>({
		...options,
		mutationFn: ({ name, departments, hardSkills }) =>
			axios
				.post(GET_SPECIALIZATION_API_ENDPOINT, {
					name,
					hardSkillids: hardSkills?.map((hardSkill) => hardSkill.id) ?? [],
					departmentIds: departments.map((department) => department.id),
				})
				.then((res) => res.data)
				.catch((error) => {
					throw error;
				}),
	});
