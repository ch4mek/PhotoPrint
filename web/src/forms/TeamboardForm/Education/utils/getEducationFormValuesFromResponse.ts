import { EducationFormValues } from '../types';

import { PersistentConfigurationItem } from '@/types/configuration';
import { EducationsResponse } from '@/types/educations';

export const getEducationFormValuesFromResponse = (
	{
		educationLevel,
		educationalInstitution,
		faculty,
		specialization,
		studyForm,
		graduationYear,
		comment,
	}: EducationsResponse,
	{
		educationLevelTypes,
		studyFormTypes,
	}: {
		educationLevelTypes: Array<PersistentConfigurationItem>;
		studyFormTypes: Array<PersistentConfigurationItem>;
	}
): EducationFormValues => ({
	educationLevel:
		educationLevelTypes.find(({ id }) => id === educationLevel) ?? null,
	educationalInstitution,
	faculty,
	specialization,
	studyForm: studyFormTypes.find(({ id }) => id === studyForm) ?? null,
	graduationYear: String(graduationYear),
	comment: comment ?? '',
});
