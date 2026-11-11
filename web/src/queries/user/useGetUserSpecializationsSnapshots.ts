import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';
import buildQuery from 'odata-query';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_SPECIALIZATIONS_SNAPSHOTS } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { getEncodedQueryString } from '@/helpers/getEncodedQueryString';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

import { UserHardSkillsResponse } from './types';

export const useGetUserSpecializationsSnapshots = (
	userId: string | number | undefined,
	page: number,
	rowsPerPage: number,
	specializationTemplateName?: string
) =>
	useQueryWithCallback<UserHardSkillsResponse>({
		queryKey: [
			TeamboardQueriesKeys.getUserSpecializationsSnapshots,
			userId,
			page,
			rowsPerPage,
			specializationTemplateName,
		],
		queryFn: async () => {
			const query = buildQuery({
				filter: {
					and: [
						specializationTemplateName &&
							`contains(tolower(specializationTemplateName), tolower('${getEncodedQueryString(
								specializationTemplateName
							)}'))`,
					],
				},
				top: rowsPerPage,
				skip: page * rowsPerPage,
				orderBy: `isPrimary ${SortDirection.DESC}, createdAtUtc ${SortDirection.DESC}`,
			});

			const { data } = await axios.get<UserHardSkillsResponse>(
				`${GET_USER_SPECIALIZATIONS_SNAPSHOTS}${query}`,
				{
					params: {
						userId,
					},
				}
			);

			return data;
		},
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
		enabled: !!userId,
	});
