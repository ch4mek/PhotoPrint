import { fromToDateObject } from '@/constants/defaultValues';
import { IS_ACTIVE_DEFAULT_VALUE } from '@/pages/constants';
import { ProjectsTableFilters } from '@/types/projectsTableFilters';

import { DateRangeModalFilter } from './types';

export const DEFAULT_DATE_RANGE_VALUE: DateRangeModalFilter = {
	fromDate: null,
	toDate: null,
	includeNull: false,
	areDatePickersActive: false,
};

export const DEFAULT_USER_PROJECTS_TAB_FILTER_VALUES: ProjectsTableFilters = {
	projectName: '',
	clientName: '',
	isActive: IS_ACTIVE_DEFAULT_VALUE,
	billability: [],
	startDate: fromToDateObject,
	endDate: fromToDateObject,
};

export const LOADER_ROWS_AMOUNT = 1;
