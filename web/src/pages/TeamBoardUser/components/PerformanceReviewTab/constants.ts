import { PerformanceReviewFilter } from '@/pages/TeamBoardUser/types';

export const DEFAULT_PERFORMANCE_FILTER_VALUES: PerformanceReviewFilter = {
	performanceReviewDate: {
		from: null,
		to: null,
	},
	agreements: '',
	employeeTheses: '',
	theses: '',
};
