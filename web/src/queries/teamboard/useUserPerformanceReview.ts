import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ByPageResponse, ServerResponseError } from '../types';
import { convertDateFromToValueToQueryParam } from '../utils/convertFromToValueToQueryParam';

import { performanceReviewQueriesKeys } from '@/constants/queriesKeys';
import { USER_PERFORMANCE_REVIEW_API_ENDPOINT } from '@/constants/queryPaths';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { UserPerformanceReviewData } from '@/types/teamboard';

import {
	GetPerformanceReviewQueryParams,
	PerformanceReviewQueryFilter,
} from './types';

const OVERDUE_DAYS_COUNT = 7;

export const useUserPerformanceReview = (
	params: GetPerformanceReviewQueryParams,
	options?: PatchedQueryOptions<
		ByPageResponse<UserPerformanceReviewData>,
		AxiosError<ServerResponseError>,
		ByPageResponse<UserPerformanceReviewData>
	>
) =>
	useQueryWithCallback({
		queryKey: [performanceReviewQueriesKeys.getUserPerformanceReview, params],
		queryFn: async () => {
			const { filters, top, skip, userId, orderBy } = params;
			const fromToPropName = 'performanceReviewDate';

			try {
				const queryFilters = filters ?? ({} as PerformanceReviewQueryFilter);
				const { performanceReviewDate, agreements, theses, employeeTheses } =
					queryFilters;

				const query = buildQuery({
					top,
					skip,
					filter: {
						and: [
							userId && `userId eq ${userId}`,
							agreements &&
								`contains(tolower(agreements), tolower('${replaceSpecialCharacters(
									agreements
								)}'))`,
							employeeTheses &&
								`contains(tolower(employeeTheses), tolower('${replaceSpecialCharacters(
									employeeTheses
								)}'))`,
							theses &&
								`contains(tolower(theses), tolower('${replaceSpecialCharacters(
									theses
								)}'))`,
							...convertDateFromToValueToQueryParam(
								performanceReviewDate,
								fromToPropName
							),
						],
					},
					orderBy,
				});

				const { data } = await axios.get<
					ByPageResponse<UserPerformanceReviewData>
				>(`${USER_PERFORMANCE_REVIEW_API_ENDPOINT}/${query}`, {
					params: {
						overdueDaysCount: OVERDUE_DAYS_COUNT,
					},
				});

				return data;
			} catch (error) {
				throw error as AxiosError<ServerResponseError>;
			}
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		...options,
	});
