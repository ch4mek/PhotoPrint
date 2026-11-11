import { FormikErrors, FormikTouched } from 'formik';

import { FormSpecialization } from '@/types/teamboard';

import { UserSpecializationsFormValues } from './types';

function isStringArray(array: string[] | unknown[]): array is string[] {
	return typeof array[0] === 'string';
}

export const extractErrors = (
	errors: FormikErrors<UserSpecializationsFormValues>
) => {
	if (
		!errors.userSpecializations ||
		typeof errors.userSpecializations === 'string' ||
		isStringArray(errors.userSpecializations)
	) {
		return [];
	}

	return errors.userSpecializations;
};

export const extractSpecializationsErrors = (
	extractedErrors: FormikErrors<FormSpecialization>[],
	index: number
) =>
	Array.isArray(extractedErrors[index]?.hardSkills) &&
	!isStringArray(extractedErrors[index]?.hardSkills)
		? extractedErrors[index]?.hardSkills
		: undefined;

export const extractSpecializationsTouched = (
	touched: FormikTouched<UserSpecializationsFormValues>,
	index: number
) =>
	Array.isArray(touched.userSpecializations?.[index]?.hardSkills)
		? (touched.userSpecializations?.[index]?.hardSkills as unknown as boolean[])
		: undefined;

export const extractSeniorityError = (
	extractedErrors: FormikErrors<FormSpecialization>[],
	index: number
) => {
	const specErrors = extractedErrors[index];

	return typeof specErrors?.seniority === 'string'
		? specErrors.seniority
		: undefined;
};
