import { DEFAULT_EDUCATIONS_FILTER_VALUES } from '../constants';

import {
	EducationsAction,
	EducationsActionKeys,
	EducationsGridFilters,
} from '@/types/educations';

export const educationsFiltersReducer = (
	state: EducationsGridFilters,
	{ type, payload }: EducationsAction
): EducationsGridFilters => {
	switch (type) {
		case EducationsActionKeys.set:
			return payload as EducationsGridFilters;
		case EducationsActionKeys.educationLevel:
			return { ...state, educationLevel: payload as string[] };
		case EducationsActionKeys.educationalInstitution:
			return { ...state, educationalInstitution: payload as string };
		case EducationsActionKeys.faculty:
			return { ...state, faculty: payload as string };
		case EducationsActionKeys.specialization:
			return { ...state, specialization: payload as string };
		case EducationsActionKeys.studyForm:
			return { ...state, studyForm: payload as string[] };
		case EducationsActionKeys.graduationYear:
			return { ...state, graduationYear: payload as string };
		case EducationsActionKeys.comment:
			return { ...state, comment: payload as string };
		case EducationsActionKeys.clear:
			return { ...DEFAULT_EDUCATIONS_FILTER_VALUES };

		default:
			return state;
	}
};
