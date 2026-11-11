import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { GET_HARD_SKILLS_API_ENDPOINT } from '@/constants/queryPaths';
import { HardSkill } from '@/types/settings';

export const useUpdateHardSkill = (
	options: UseMutationOptions<
		HardSkill,
		AxiosError<ServerResponseError>,
		HardSkill
	>
) =>
	useMutation<HardSkill, AxiosError<ServerResponseError>, HardSkill>({
		...options,
		mutationFn: ({ name, id, hardSkillGroupId }) =>
			axios
				.put(GET_HARD_SKILLS_API_ENDPOINT, {
					name,
					id,
					hardSkillGroupId,
				})
				.then((res) => res.data)
				.catch((error) => {
					throw error;
				}),
	});
