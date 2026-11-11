import { EducationsGridFilters } from '@/types/educations';

import { EducationsGridColumnWidths } from './types';

export const DEFAULT_EDUCATIONS_FILTER_VALUES: EducationsGridFilters = {
	educationLevel: [],
	educationalInstitution: '',
	faculty: '',
	specialization: '',
	studyForm: [],
	graduationYear: '',
	comment: '',
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
	comment: 200,
};

export const EDUCATIONS_GRID_COLUMNS = {
	educationLevel: {
		key: 'educationLevel',
		name: 'Education level',
		sortable: true,
	},
	educationalInstitution: {
		key: 'educationalInstitution',
		name: 'Educational institution',
		sortable: true,
	},
	faculty: {
		key: 'faculty',
		name: 'Faculty',
		sortable: true,
	},
	specialization: {
		key: 'specialization',
		name: 'Specialization',
		sortable: true,
	},
	studyForm: {
		key: 'studyForm',
		name: 'Study form',
		sortable: true,
	},
	graduationYear: {
		key: 'graduationYear',
		name: 'Graduation year',
		sortable: true,
	},
	comment: {
		key: 'comment',
		name: 'Comment',
		sortable: false,
	},
};

export const DEFAULT_ORDER_BY_FIELD = 'graduationYear';

export const CLEAR_FILTERS_COLUMN_INDEX = 9;
