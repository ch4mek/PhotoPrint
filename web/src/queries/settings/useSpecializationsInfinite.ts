import { useInfiniteQuery } from '@tanstack/react-query';
import axios from 'axios';
import buildQuery from 'odata-query';

import { ByPageResponse } from '../types';

import { DEFAULT_ROWS_PER_PAGE_INT } from '@/constants/pagination';
import { SettingsSkillsQueriesKeys } from '@/constants/queriesKeys';
import { GET_SPECIALIZATION_API_ENDPOINT } from '@/constants/queryPaths';
import { DefaultSortFields, SortDirection } from '@/constants/sort';
import { capitalizeString } from '@/helpers/capitalizeString';
import { Specialization } from '@/types/settings';

interface UseSpecializationsInfiniteParams {
	departmentId: string | null;
	minHardSkillsCount?: number;
}

export const useSpecializationsInfinite = ({
	departmentId,
	minHardSkillsCount,
}: UseSpecializationsInfiniteParams) =>
	useInfiniteQuery({
		queryKey: [SettingsSkillsQueriesKeys.specializations, departmentId],
		queryFn: async ({ pageParam = 0 }) => {
			const query = buildQuery({
				top: DEFAULT_ROWS_PER_PAGE_INT,
				skip: pageParam * DEFAULT_ROWS_PER_PAGE_INT,
				orderBy: `${capitalizeString(DefaultSortFields.ID)} ${
					SortDirection.DESC
				}`,
				filter: {
					hardSkillsCount: {
						gt: minHardSkillsCount && minHardSkillsCount - 1,
					},
				},
			});

			const res = await axios.get<ByPageResponse<Specialization>>(
				`${GET_SPECIALIZATION_API_ENDPOINT}${query}`,
				{
					params: {
						departmentId,
					},
				}
			);

			return res.data;
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
