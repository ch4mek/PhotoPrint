import { useMutation, MutationObserverOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { GET_HARD_SKILLS_API_ENDPOINT } from '@/constants/queryPaths';
import { HardSkill } from '@/types/settings';

interface CreateHardSkillOption {
	name: string;
	hardSkillGroupId: string;
}

export const useCreateHardSkill = (
	options: MutationObserverOptions<
		HardSkill,
		AxiosError<ServerResponseError>,
		CreateHardSkillOption
	>
) =>
	useMutation<
		HardSkill,
		AxiosError<ServerResponseError>,
		CreateHardSkillOption
	>({
		...options,
		mutationFn: ({ name, hardSkillGroupId }) =>
			axios
				.post(GET_HARD_SKILLS_API_ENDPOINT, {
					name,
					hardSkillGroupId,
				})
				.then((res) => res.data)
				.catch((error) => {
					throw error;
				}),
	});
