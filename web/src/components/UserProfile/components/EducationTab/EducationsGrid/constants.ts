import { EducationsGridFilters } from '@/types/educations';

import { EducationsGridColumnWidths } from './types';

export const DEFAULT_EDUCATIONS_FILTER_VALUES: EducationsGridFilters = {
	educationLevel: [],
	educationalInstitution: '',
	faculty: '',
	specialization: '',
	studyForm: [],
	graduationYear: '',
};

export const columnWidths: EducationsGridColumnWidths = {
	educationLevel: {
		width: 150,
		fixed: true,
	},
	educationalInstitution: 200,
	faculty: 200,
	specialization: 200,
	studyForm: 121,
	graduationYear: 140,
};

export const EDUCATIONS_GRID_COLUMNS = {
	educationLevel: {
		key: 'educationLevel',
		sortable: true,
	},
	educationalInstitution: {
		key: 'educationalInstitution',
		sortable: true,
	},
	faculty: {
		key: 'faculty',
		sortable: true,
	},
	specialization: {
		key: 'specialization',
		sortable: true,
	},
	studyForm: {
		key: 'studyForm',
		sortable: true,
	},
	graduationYear: {
		key: 'graduationYear',
		sortable: true,
	},
};
export const DEFAULT_ORDER_BY_FIELD = 'graduationYear';

export const CLEAR_FILTERS_COLUMN_INDEX = 8;
