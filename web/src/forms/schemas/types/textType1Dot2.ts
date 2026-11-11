import * as yup from 'yup';

import { TEXT_TYPE_1_2_REG_EXP } from '../constants';

import {
	FIELD_IS_REQUIRED,
	getMaxLengthErrorMessage,
	getMinLengthErrorMessage,
	getPleaseEnterAValidFieldErrorMessage,
} from '@/constants/errors';
import {
	checkEmptyField,
	checkIfNoLettersOrNumbers,
	checkToAllHyphens,
	checkToAllHyphensAndSpaces,
	checkToAllSpaces,
} from '@/forms/utils';
import { trimmedEmptyValidityTest } from '@/helpers/trimmedEmptyValidityTest';

export const textType1Dot2Schema = (fieldName: string, isMandatory = false) => {
	const NOT_VALID_FIELD = getPleaseEnterAValidFieldErrorMessage(fieldName);
	const MIN_CHARACTERS = 2;
	const MAX_CHARACTERS = 250;

	const afterTransform = (schema: yup.StringSchema) => {
		const withTrimAndEmptyValidityCheck = schema
			.transform((value) => (value ? value.trim() : value))
			.test(
				'trimmed-empty-validity',
				NOT_VALID_FIELD,
				trimmedEmptyValidityTest
			);

		const withRequiredCheck = isMandatory
			? withTrimAndEmptyValidityCheck.required(FIELD_IS_REQUIRED)
			: withTrimAndEmptyValidityCheck;

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
			.nullable()
			.test('only-symbols-check', NOT_VALID_FIELD, (value) => {
				if (value?.length) {
					return !checkIfNoLettersOrNumbers(value);
				}

				return !checkEmptyField(value);
			})
			.matches(TEXT_TYPE_1_2_REG_EXP, NOT_VALID_FIELD)
			.min(MIN_CHARACTERS, getMinLengthErrorMessage(MIN_CHARACTERS))
			.max(MAX_CHARACTERS, getMaxLengthErrorMessage(MAX_CHARACTERS));
	};

	return afterTransform(yup.string());
};
