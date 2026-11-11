import * as yup from 'yup';

import { TEXT_TYPE_2_REG_EXP } from '../constants';

import {
	FIELD_IS_REQUIRED,
	getMaxLengthErrorMessage,
	getMinLengthErrorMessage,
	getPleaseEnterAValidFieldErrorMessage,
} from '@/constants/errors';
import {
	checkIfNoLettersOrNumbers,
	checkToAllHyphens,
	checkToAllHyphensAndSpaces,
	checkToAllSpaces,
} from '@/forms/utils';
import { getTrimedString } from '@/helpers/getTrimedString';
import { trimmedEmptyValidityTest } from '@/helpers/trimmedEmptyValidityTest';

export const textType2Schema = (fieldName: string) => {
	const NOT_VALID_FIELD = getPleaseEnterAValidFieldErrorMessage(fieldName);
	const MIN_CHARACTERS = 2;
	const MAX_CHARACTERS = 250;

	return yup
		.string()
		.transform((value) => (value ? getTrimedString(value) : value))
		.test('trimmed-empty-validity', NOT_VALID_FIELD, trimmedEmptyValidityTest)
		.required(FIELD_IS_REQUIRED)
		.test('all-spaces-check', NOT_VALID_FIELD, checkToAllSpaces)
		.test('all-hyphens-check', NOT_VALID_FIELD, (value) =>
			checkToAllHyphens(value)
		)
		.test('all-hyphens-and-spaces-check', NOT_VALID_FIELD, (value) =>
			checkToAllHyphensAndSpaces(value)
		)
		.test(
			'only-symbols-check',
			NOT_VALID_FIELD,
			(value) => !checkIfNoLettersOrNumbers(value)
		)
		.matches(TEXT_TYPE_2_REG_EXP, NOT_VALID_FIELD)
		.min(MIN_CHARACTERS, getMinLengthErrorMessage(MIN_CHARACTERS))
		.max(MAX_CHARACTERS, getMaxLengthErrorMessage(MAX_CHARACTERS));
};
