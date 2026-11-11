import { MIN_FULLNAME_COLUMN_WIDTH } from '@/constants/dashboard';

const DATES_COLUMN_WIDTH = 115;

export const PERFORMANCE_GRID_COLUMNS = {
	performanceFullNameKey: 'fullName',
	performanceDatesKey: 'dates',
};

export const performanceReviewColumnWidth = {
	fullName: {
		width: MIN_FULLNAME_COLUMN_WIDTH,
	},
	dates: {
		width: DATES_COLUMN_WIDTH,
		fixed: true,
	},
};
