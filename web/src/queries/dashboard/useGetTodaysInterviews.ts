import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { dashboardQueriesKeys } from '@/constants/queriesKeys';
import { TODAYS_INTERVIEWS_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

import { GetTodaysInterviewsResponse, UseGetDashboardParams } from './types';

export const useGetTodaysInterviews = ({
	departmentId,
}: UseGetDashboardParams) =>
	useQueryWithCallback<GetTodaysInterviewsResponse, AxiosError>({
		queryKey: [dashboardQueriesKeys.getTodaysInterviewsLookup, departmentId],
		queryFn: async () =>
			axios
				.get<GetTodaysInterviewsResponse>(TODAYS_INTERVIEWS_API_ENDPOINT, {
					params: {
						departmentId,
					},
				})
				.then((res) => res.data),
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
	});
