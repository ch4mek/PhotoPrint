import { useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { PERFORMANCE_REVIEW_API_ENDPOINT } from '@/constants/queryPaths';

import {
	CreatePerformanceReviewResponse,
	CreatePerformanceReviewOptions,
	UseCreatePerformanceReviewOptions,
} from './useCreatePerformanceReview.types';

export const useCreatePerformanceReview = (
	options: UseCreatePerformanceReviewOptions
) =>
	useMutation<
		CreatePerformanceReviewResponse,
		AxiosError<ServerResponseError>,
		CreatePerformanceReviewOptions
	>({
		...options,

		mutationFn: async ({ userId, date }) =>
			axios
				.post(PERFORMANCE_REVIEW_API_ENDPOINT, { userId, date })
				.then((res) => res.data)
				.catch((error: AxiosError<ServerResponseError>) => {
					throw error;
				}),
	});
