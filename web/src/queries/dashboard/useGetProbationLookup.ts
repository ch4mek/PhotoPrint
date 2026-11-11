import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { dashboardQueriesKeys } from '@/constants/queriesKeys';
import { PROBATION_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

import { GetProbationResponse, UseGetDashboardParams } from './types';

export const useGetProbationLookup = ({
	departmentId,
}: UseGetDashboardParams) =>
	useQueryWithCallback<GetProbationResponse, AxiosError>({
		queryKey: [dashboardQueriesKeys.getProbationLookup, departmentId],
		queryFn: async () =>
			axios
				.get<GetProbationResponse>(PROBATION_API_ENDPOINT, {
					params: {
						departmentId,
					},
				})
				.then((res) => res.data),
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
	});
