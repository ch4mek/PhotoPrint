import { EducationFormValues } from './types';

export const EDUCATION_TITLE = 'Education';

export const EMPTY_FORM_VALUES: EducationFormValues = {
	educationLevel: null,
	educationalInstitution: '',
	faculty: '',
	specialization: '',
	studyForm: null,
	graduationYear: '',
	comment: '',
};

export const EDUCATION_FORM_FIELDS = {
	educationLevel: {
		label: 'Education level',
		name: 'educationLevel',
	},
	educationalInstitution: {
		label: 'Educational institution',
		name: 'educationalInstitution',
	},
	faculty: {
		label: 'Faculty',
		name: 'faculty',
	},
	specialization: {
		label: 'Specialization',
		name: 'specialization',
	},
	studyForm: {
		label: 'Study form',
		name: 'studyForm',
	},
	graduationYear: {
		label: 'Graduation year',
		name: 'graduationYear',
	},
	comment: {
		label: 'Comment',
		name: 'comment',
	},
};
