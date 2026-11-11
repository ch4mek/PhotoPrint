import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';

import { GET_SOFT_SKILLS_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { SoftSkill } from '@/types/settings';

export const useSoftSkill = (id: number | string | undefined) =>
	useQueryWithCallback({
		queryKey: ['soft-skills', id],
		queryFn: () =>
			axios
				.get<SoftSkill>(`${GET_SOFT_SKILLS_API_ENDPOINT}/${id}`)
				.then((res) => res.data),

		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		enabled: Boolean(id),
	});
