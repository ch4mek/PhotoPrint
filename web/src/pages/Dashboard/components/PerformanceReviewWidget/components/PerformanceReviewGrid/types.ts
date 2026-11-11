import { GetPerformanceReviewResponse } from '@/queries/dashboard/types';
import { PerformanceReviewDate } from '@/types/performanceReview';

export interface PerformanceReviewGridRow {
	id: number;
	userId: number;
	fullName: string;
	dates: PerformanceReviewDate[];
}

export interface PerformanceReviewGridComponentProps {
	data: GetPerformanceReviewResponse;
}
