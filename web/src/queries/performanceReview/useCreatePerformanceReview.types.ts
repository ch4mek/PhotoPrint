import { UseMutationOptions } from '@tanstack/react-query';
import { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

export interface CreatePerformanceReviewResponse {
	userId: number;
	date: string;
}

export interface CreatePerformanceReviewOptions {
	userId: number;
	date: string;
}

export type UseCreatePerformanceReviewOptions = UseMutationOptions<
	CreatePerformanceReviewResponse,
	AxiosError<ServerResponseError>,
	CreatePerformanceReviewOptions
>;
