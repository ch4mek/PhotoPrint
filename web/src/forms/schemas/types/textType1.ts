import * as yup from 'yup';

import { TEXT_TYPE_1_REG_EXP } from '../constants';

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

export const textType1Schema = (
	fieldName: string,
	disableRequired?: boolean
) => {
	const NOT_VALID_FIELD = getPleaseEnterAValidFieldErrorMessage(fieldName);
	const MIN_CHARACTERS = 2;
	const MAX_CHARACTERS = 250;

	const afterTransform = (schema: yup.StringSchema) => {
		const withTrimAndEmptyValidityCheck = schema
			.transform((value) => (value ? getTrimedString(value) : value))
			.test(
				'trimmed-empty-validity',
				NOT_VALID_FIELD,
				trimmedEmptyValidityTest
			);

		const withRequiredCheck = disableRequired
			? withTrimAndEmptyValidityCheck
			: withTrimAndEmptyValidityCheck.required(FIELD_IS_REQUIRED);

		return withRequiredCheck
			.test('all-spaces-check', NOT_VALID_FIELD, (value) =>
				checkToAllSpaces(value)
			)
			.test('all-hyphens-check', NOT_VALID_FIELD, (value) =>
				checkToAllHyphens(value)
			)
			.test('all-hyphens-and-spaces-check', NOT_VALID_FIELD, (value) =>
				checkToAllHyphensAndSpaces(value)
			)
			.test('only-symbols-check', NOT_VALID_FIELD, (value) =>
				disableRequired ? true : !checkIfNoLettersOrNumbers(value)
			)
			.matches(TEXT_TYPE_1_REG_EXP, NOT_VALID_FIELD)
			.min(MIN_CHARACTERS, getMinLengthErrorMessage(MIN_CHARACTERS))
			.max(MAX_CHARACTERS, getMaxLengthErrorMessage(MAX_CHARACTERS));
	};

	return afterTransform(yup.string());
};
