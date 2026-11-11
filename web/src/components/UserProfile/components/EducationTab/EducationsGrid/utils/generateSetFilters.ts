import { Dispatch } from 'react';

import {
	EducationsAction,
	EducationsActionKeys,
	EducationsGridFilters,
	EducationsGridFilterSetters,
} from '@/types/educations';

export const generateSetFilters = (
	dispatch: Dispatch<EducationsAction>
): EducationsGridFilterSetters => ({
	educationLevel: (educationLevel: string[]) =>
		dispatch({
			type: EducationsActionKeys.educationLevel,
			payload: educationLevel,
		}),
	educationalInstitution: (educationalInstitution: string) =>
		dispatch({
			type: EducationsActionKeys.educationalInstitution,
			payload: educationalInstitution,
		}),
	faculty: (faculty: string) =>
		dispatch({ type: EducationsActionKeys.faculty, payload: faculty }),
	specialization: (specialization: string) =>
		dispatch({
			type: EducationsActionKeys.specialization,
			payload: specialization,
		}),
	studyForm: (studyForm: string[]) =>
		dispatch({ type: EducationsActionKeys.studyForm, payload: studyForm }),
	graduationYear: (graduationYear: string) =>
		dispatch({
			type: EducationsActionKeys.graduationYear,
			payload: graduationYear,
		}),
	set: (newFilters: EducationsGridFilters) =>
		dispatch({ type: EducationsActionKeys.set, payload: newFilters }),
	clear: () => dispatch({ type: EducationsActionKeys.clear }),
});
