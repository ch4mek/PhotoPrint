import { useMutation, MutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { GET_SOFT_SKILLS_API_ENDPOINT } from '@/constants/queryPaths';
import { SoftSkill } from '@/types/settings';

export const useCreateSoftSkill = (
	options: MutationOptions<
		SoftSkill,
		AxiosError<ServerResponseError>,
		SoftSkill
	>
) =>
	useMutation<SoftSkill, AxiosError<ServerResponseError>, SoftSkill>({
		...options,
		mutationFn: ({ name }) =>
			axios
				.post(GET_SOFT_SKILLS_API_ENDPOINT, {
					name,
				})
				.then((res) => res.data)
				.catch((error) => {
					throw error;
				}),
	});
