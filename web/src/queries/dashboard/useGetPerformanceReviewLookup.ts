import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { OVERDUE_DAYS_COUNT, UPPER_DAYS_BORDER } from '@/constants/dashboard';
import { dashboardQueriesKeys } from '@/constants/queriesKeys';
import { PERFORMANCE_REVIEW_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

import { GetPerformanceReviewResponse, UseGetDashboardParams } from './types';

export const useGetPerformanceReviewLookup = ({
	departmentId,
}: UseGetDashboardParams) =>
	useQueryWithCallback<GetPerformanceReviewResponse, AxiosError>({
		queryKey: [dashboardQueriesKeys.getPerformanceReviewLookup, departmentId],
		queryFn: async () =>
			axios
				.get<GetPerformanceReviewResponse>(
					`${PERFORMANCE_REVIEW_API_ENDPOINT}/lookup`,
					{
						params: {
							overdueDays: OVERDUE_DAYS_COUNT,
							upperDaysBorder: UPPER_DAYS_BORDER,
							departmentId,
						},
					}
				)
				.then((res) => res.data),
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
	});
