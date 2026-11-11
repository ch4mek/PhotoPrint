import { FormikErrors } from 'formik';

import { SeniorityFormsValues } from '@/forms/TeamboardForm/SeniorityForm/types';
import { isStringArray } from '@/helpers/typePredicates';

export const extractSkillsErrors = (
	errors: FormikErrors<SeniorityFormsValues>
) => {
	if (
		!errors.skills ||
		typeof errors.skills === 'string' ||
		isStringArray(errors.skills)
	) {
		return [];
	}

	return errors.skills;
};
