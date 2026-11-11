import { MIN_FULLNAME_COLUMN_WIDTH } from '@/constants/dashboard';

const DATES_COLUMN_WIDTH = 100;

export const PROBATION_GRID_COLUMNS = {
	probationFullNameKey: 'userFullName',
	probationDatesKey: 'date',
};

export const probationColumnWidth = {
	userFullName: {
		width: MIN_FULLNAME_COLUMN_WIDTH,
	},
	date: {
		width: DATES_COLUMN_WIDTH,
		fixed: true,
	},
};
