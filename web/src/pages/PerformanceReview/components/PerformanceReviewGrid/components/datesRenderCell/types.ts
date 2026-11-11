import { PerformanceReviewUserData } from '@/types/performanceReview';

export type HandleTagChoose = (
	userData: PerformanceReviewUserData,
	date: string,
	performanceReviewId: number,
	conversationId?: number
) => () => void;
