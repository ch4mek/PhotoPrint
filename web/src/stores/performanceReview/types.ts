import { PerformanceReviewUserData } from '@/types/performanceReview';

export interface PerformanceReviewStore {
	users: PerformanceReviewUserData[];
	setUsers: (users: PerformanceReviewUserData[]) => void;
	isShowIssuedStatus: boolean;
	toggleIsShowIssuedStatus: VoidFunction;
}
