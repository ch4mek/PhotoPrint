import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';

import { GET_HARD_SKILLS_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { HardSkill } from '@/types/settings';

export const useHardSkill = (id: number | string) =>
	useQueryWithCallback({
		queryKey: ['hard-skill', id],
		queryFn: () =>
			axios
				.get<HardSkill>(`${GET_HARD_SKILLS_API_ENDPOINT}/${id}`)
				.then((res) => res.data),

		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		enabled: Boolean(id),
	});
