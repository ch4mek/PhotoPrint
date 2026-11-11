import { useInfiniteQuery } from '@tanstack/react-query';
import axios from 'axios';
import buildQuery from 'odata-query';

import { DEFAULT_ROWS_PER_PAGE_INT } from '@/constants/pagination';
import { SettingsSkillsQueriesKeys } from '@/constants/queriesKeys';
import { GET_SOFT_SKILLS_API_ENDPOINT } from '@/constants/queryPaths';
import { DefaultSortFields, SortDirection } from '@/constants/sort';
import { capitalizeString } from '@/helpers/capitalizeString';
import { SoftSkillResponse } from '@/types/settings';

export const useSoftSkillsInfinite = () =>
	useInfiniteQuery({
		queryKey: [SettingsSkillsQueriesKeys.softSkills],
		queryFn: async ({ pageParam = 0 }) => {
			const query = buildQuery({
				top: DEFAULT_ROWS_PER_PAGE_INT,
				skip: pageParam * DEFAULT_ROWS_PER_PAGE_INT,
				orderBy: `${capitalizeString(DefaultSortFields.ID)} ${
					SortDirection.DESC
				}`,
			});

			const response = await axios.get<SoftSkillResponse>(
				`${GET_SOFT_SKILLS_API_ENDPOINT}${query}`
			);

			return response.data;
		},
		getNextPageParam: (lastPage, allPages) => {
			const loadedItems = allPages.reduce(
				(sum, page) => sum + page.items.length,
				0
			);

			return loadedItems < lastPage.total ? allPages.length : undefined;
		},
		initialPageParam: 0,
		refetchOnWindowFocus: false,
	});
