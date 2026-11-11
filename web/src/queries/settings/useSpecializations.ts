import axios from 'axios';
import buildQuery from 'odata-query';

import { ByPageResponse } from '../types';

import { SettingsSkillsQueriesKeys } from '@/constants/queriesKeys';
import { GET_SPECIALIZATION_API_ENDPOINT } from '@/constants/queryPaths';
import { DefaultSortFields, SortDirection } from '@/constants/sort';
import { capitalizeString } from '@/helpers/capitalizeString';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { Specialization } from '@/types/settings';

export const useSpecializations = (
	page: number,
	rowsPerPage: number,
	departmentId: string | null,
	minHardSkillsCount?: number
) =>
	useQueryWithCallback({
		queryKey: [SettingsSkillsQueriesKeys.specializations, page, departmentId],
		queryFn: async () => {
			const query = buildQuery({
				top: rowsPerPage,
				skip: page * rowsPerPage,
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
		refetchOnWindowFocus: false,
	});
