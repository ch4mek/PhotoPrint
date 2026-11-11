import { MIN_FULLNAME_COLUMN_WIDTH } from '@/constants/dashboard';

const SPECIALIZATION_COLUMN_WIDTH = 100;
const TIME_COLUMN_WIDTH = 60;

export const INTERVIEW_GRID_COLUMNS = {
	interviewSpecializationKey: 'specialization',
	interviewFullNameKey: 'userFullName',
	interviewTimeKey: 'time',
};

export const interviewColumnWidth = {
	specialization: {
		width: SPECIALIZATION_COLUMN_WIDTH,
		fixed: true,
	},
	fullName: {
		width: MIN_FULLNAME_COLUMN_WIDTH,
	},
	time: {
		width: TIME_COLUMN_WIDTH,
		fixed: true,
	},
};
