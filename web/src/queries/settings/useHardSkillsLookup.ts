import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { GET_HARD_SKILLS_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { HardSkill } from '@/types/settings';

export const useHardSkillsLookup = (enabled: boolean) =>
	useQueryWithCallback<Array<HardSkill>, AxiosError<ServerResponseError>>({
		queryKey: ['hard-skills-lookup'],
		queryFn: () =>
			axios
				.get<Array<HardSkill>>(`${GET_HARD_SKILLS_API_ENDPOINT}/lookup`)
				.then((res) => res.data),
		refetchOnWindowFocus: false,
		enabled,
	});
