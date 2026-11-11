import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { GET_SOFT_SKILLS_API_ENDPOINT } from '@/constants/queryPaths';
import { SoftSkill } from '@/types/settings';

export const useUpdateSoftSkill = (
	options: UseMutationOptions<
		SoftSkill,
		AxiosError<ServerResponseError>,
		SoftSkill
	>
) =>
	useMutation<SoftSkill, AxiosError<ServerResponseError>, SoftSkill>({
		...options,
		mutationFn: ({ name, id }) =>
			axios
				.put(GET_SOFT_SKILLS_API_ENDPOINT, {
					name,
					id,
				})
				.then((res) => res.data)
				.catch((error) => {
					throw error;
				}),
	});
