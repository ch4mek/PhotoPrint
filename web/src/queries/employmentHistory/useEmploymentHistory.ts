import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_EMPLOYMENT_HISTORY_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { EmploymentHistoryResponse } from '@/types/employmentHistory';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useEmploymentHistory = (
	id: string | number | undefined,
	queryOptions?: PatchedQueryOptions<EmploymentHistoryResponse, AxiosError>
) =>
	useQueryWithCallback<EmploymentHistoryResponse, AxiosError>({
		...queryOptions,
		queryKey: [TeamboardQueriesKeys.employmentHistory, id],
		queryFn: async () => {
			const { data } = await axios.get<EmploymentHistoryResponse>(
				`${GET_EMPLOYMENT_HISTORY_API_ENDPOINT}`,
				{
					params: {
						userId: id,
					},
				}
			);

			return data;
		},
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
		enabled: Boolean(id),
	});
