import { FormikErrors } from 'formik';

import { isStringArray } from '@/helpers/typePredicates';
import {
	FormSpecialization,
	UserNotAssignedSpecialization,
	UserSpecializationFormValues,
} from '@/types/teamboard';

export const convertNotAssignedSpecializationsToOptions = (
	specializations: Array<UserNotAssignedSpecialization>
): Array<FormSpecialization> =>
	specializations.map((specialization) => {
		const { specializationTemplateId, specializationTemplateName, hardSkills } =
			specialization;

		return {
			id: specializationTemplateId,
			name: specializationTemplateName,
			hardSkills: hardSkills.map((hardSkill) => ({
				info: hardSkill,
				seniority: null,
			})),
			seniority: null,
		};
	});

export const extractSpecializationErrors = (
	errors: FormikErrors<UserSpecializationFormValues>
) => {
	if (
		!errors.specialization ||
		typeof errors.specialization === 'string' ||
		isStringArray(errors.specialization)
	) {
		return [];
	}

	if ('hardSkills' in errors.specialization) {
		return (errors.specialization as any).hardSkills;
	}

	return errors.specialization;
};
