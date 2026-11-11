import * as yup from 'yup';

import {
	FIELD_IS_REQUIRED,
	NOT_VALID_PHONE_FORMAT,
	getMaxLengthErrorMessage,
} from '@/constants/errors';
import { PHONE_REG_EXP } from '@/constants/regexp';
import {
	checkIfNoLettersOrNumbers,
	checkToAllHyphens,
	checkToAllHyphensAndSpaces,
	checkToAllSpaces,
} from '@/forms/utils';

export const phoneNumberType1Schema = (isRequired = true) => {
	const MAX_CHARACTERS = 15;

	const baseSchema = yup
		.string()
		.max(MAX_CHARACTERS, getMaxLengthErrorMessage(MAX_CHARACTERS))
		.test('all-spaces-check', NOT_VALID_PHONE_FORMAT, (value) =>
			!value ? true : checkToAllSpaces(value)
		)
		.test('all-hyphens-check', NOT_VALID_PHONE_FORMAT, (value) =>
			!value ? true : checkToAllHyphens(value)
		)
		.test('all-hyphens-and-spaces-check', NOT_VALID_PHONE_FORMAT, (value) =>
			!value ? true : checkToAllHyphensAndSpaces(value)
		)
		.test('only-symbols-check', NOT_VALID_PHONE_FORMAT, (value) =>
			!value ? true : !checkIfNoLettersOrNumbers(value)
		)
		.matches(PHONE_REG_EXP, NOT_VALID_PHONE_FORMAT);

	return isRequired
		? baseSchema.required(FIELD_IS_REQUIRED)
		: baseSchema.nullable().optional();
};
