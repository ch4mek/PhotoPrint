import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { performanceReviewQueriesKeys } from '@/constants/queriesKeys';
import { PERFORMANCE_REVIEW_API_ENDPOINT } from '@/constants/queryPaths';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { convertFilterArrayToQueryParam } from '@/queries/utils';

import {
	GetPerformanceReviewResponse,
	UseGetPerformanceReviewParams,
} from './useGetPerformanceReview.types';

const OVERDUE_DAYS_COUNT = 7;

export const useGetPerformanceReview = ({
	top,
	page,
	order,
	orderBy,
	filters,
	allDepartments,
	issuedOnly = false,
}: UseGetPerformanceReviewParams) =>
	useQueryWithCallback<GetPerformanceReviewResponse, AxiosError>({
		queryKey: [
			performanceReviewQueriesKeys.getAllPerformanceReviews,
			top,
			page,
			order,
			orderBy,
			filters,
			issuedOnly,
		],
		queryFn: async () => {
			const { id, departments, fullName } = filters;

			const query = buildQuery({
				filter: {
					and: [
						fullName
							? `contains(tolower(fullName), tolower('${replaceSpecialCharacters(
									fullName
							  )}'))`
							: undefined,
						id &&
							`cast(id, Edm.String) eq cast('${replaceSpecialCharacters(
								id
							)}', Edm.String)`,
						{
							departmentId: convertFilterArrayToQueryParam(
								departments,
								allDepartments.length
							),
						},
					],
				},
			});

			return axios
				.get<GetPerformanceReviewResponse>(
					`${PERFORMANCE_REVIEW_API_ENDPOINT}${query}`,
					{
						params: {
							overdueDaysCount: OVERDUE_DAYS_COUNT,
							top,
							skip: page * top,
							orderBy: `${orderBy} ${order}`,
							issuedOnly,
						},
					}
				)
				.then((res) => res.data);
		},
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
	});
