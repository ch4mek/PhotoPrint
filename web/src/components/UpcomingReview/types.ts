import { TagTypes } from '../DateTag/types';

export interface UpcomingReviewProps {
	isFetching: boolean;
	isLoading: boolean;
	upcomingPerfomanceReview: string | null;
	performanceReviewDateStatus: TagTypes | null;
}
