import { RELATIVE_EDIT_COLUMN_CELL_CLASS } from '@/components/Grid/constants';
import { isChiefRole, Roles } from '@/constants/roles';
import { PerformanceReviewTableFilters } from '@/pages/PerformanceReview/components/PerformanceReviewGrid/types';

const ID_COLUMN_WIDTH = 67;
const DEPARTMENTS_COLUMN_WIDTH = 165;
const FULL_NAME_COLUMN_WIDTH = 260;

export const PERFORMANCE_GRID_COLUMNS = {
	performanceId: {
		key: 'id',
		name: 'Id',
		sortable: true,
	},
	performanceFullName: {
		key: 'fullName',
		name: 'User',
		sortable: true,
	},
	performanceUserDepartment: {
		key: 'departmentfullname',
		name: 'Department',
		sortable: true,
	},
	performanceDates: {
		key: 'dates',
		name: 'Dates',
		sortable: false,
	},
	performanceFilter: {
		key: '',
		name: '',
		frozen: true,
		sortable: false,
		cellClass: RELATIVE_EDIT_COLUMN_CELL_CLASS,
	},
};

export const defaultPerformanceReviewFilterValues: PerformanceReviewTableFilters =
	{
		id: '',
		fullName: '',
		departments: [],
	};

export const getPerformanceReviewColumnWidth = (role: Roles) => ({
	id: {
		width: ID_COLUMN_WIDTH,
		fixed: true,
	},
	fullName: {
		width: FULL_NAME_COLUMN_WIDTH,
		fixed: true,
	},
	departments: {
		width: isChiefRole(role) ? DEPARTMENTS_COLUMN_WIDTH : 0,
		fixed: true,
	},
	dates: '1fr',
});

export const getPerformanceReviewColumnsAmount = (role: Roles) =>
	Object.keys(getPerformanceReviewColumnWidth(role)).length;
