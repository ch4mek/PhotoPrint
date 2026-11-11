import { useInfiniteQuery } from '@tanstack/react-query';
import axios from 'axios';
import buildQuery from 'odata-query';

import { DEFAULT_ROWS_PER_PAGE_INT } from '@/constants/pagination';
import { SettingsSkillsQueriesKeys } from '@/constants/queriesKeys';
import { GET_HARD_SKILLS_API_ENDPOINT } from '@/constants/queryPaths';
import { DefaultSortFields, SortDirection } from '@/constants/sort';
import { capitalizeString } from '@/helpers/capitalizeString';
import { HardSkillResponse } from '@/types/settings';

interface UseHardSkillsInfiniteParams {
	departmentId: string | null;
	specializationId?: number;
}

export const useHardSkillsInfinite = ({
	departmentId,
	specializationId,
}: UseHardSkillsInfiniteParams) =>
	useInfiniteQuery({
		queryKey: [
			SettingsSkillsQueriesKeys.hardSkills,
			departmentId,
			specializationId,
		],
		queryFn: ({ pageParam = 0 }) => {
			const query = buildQuery({
				top: DEFAULT_ROWS_PER_PAGE_INT,
				skip: pageParam * DEFAULT_ROWS_PER_PAGE_INT,
				orderBy: `${capitalizeString(DefaultSortFields.ID)} ${
					SortDirection.DESC
				}`,
			});

			return axios
				.get<HardSkillResponse>(`${GET_HARD_SKILLS_API_ENDPOINT}${query}`, {
					params: {
						departmentId,
						specializationTemplateId: specializationId,
					},
				})
				.then((res) => res.data);
		},
		getNextPageParam: (lastPage, allPages) => {
			if (lastPage.items.length === DEFAULT_ROWS_PER_PAGE_INT) {
				return allPages.length;
			}

			return undefined;
		},
		initialPageParam: 0,
		refetchOnWindowFocus: false,
	});
