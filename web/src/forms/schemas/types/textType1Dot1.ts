import * as yup from 'yup';

import { TEXT_TYPE_1_REG_EXP } from '../constants';

import {
	FIELD_IS_REQUIRED,
	getMaxLengthErrorMessage,
	getMinLengthErrorMessage,
	getPleaseEnterAValidFieldErrorMessage,
} from '@/constants/errors';
import {
	checkIfThereIsNoEnoughChars,
	checkIfNoLettersOrNumbers,
	checkToAllHyphens,
	checkToAllHyphensAndSpaces,
	checkToAllSpaces,
} from '@/forms/utils';
import { getTrimedString } from '@/helpers/getTrimedString';
import { trimmedEmptyValidityTest } from '@/helpers/trimmedEmptyValidityTest';

export const textType1Dot1Schema = (fieldName: string, isMandatory = true) => {
	const NOT_VALID_FIELD = getPleaseEnterAValidFieldErrorMessage(fieldName);
	const MAX_CHARACTERS = 501;
	const MIN_CHARACTERS = 5;

	const afterTransform = (schema: yup.StringSchema) => {
		const withTrimAndEmptyValidityCheck = schema
			.nullable()
			.transform((value) => {
				if (value === '' && !isMandatory) {
					return null;
				}

				return value ? getTrimedString(value) : value;
			})
			.test(
				'trimmed-empty-validity',
				NOT_VALID_FIELD,
				trimmedEmptyValidityTest
			);

		const withRequiredCheck = isMandatory
			? withTrimAndEmptyValidityCheck.required(FIELD_IS_REQUIRED)
			: withTrimAndEmptyValidityCheck;

		return withRequiredCheck
			.test(
				'valid-symbols-count',
				getMinLengthErrorMessage(MIN_CHARACTERS),
				(value) => checkIfThereIsNoEnoughChars(MIN_CHARACTERS, value)
			)
			.test('all-spaces-check', NOT_VALID_FIELD, (value) =>
				checkToAllSpaces(value)
			)
			.test('all-hyphens-check', NOT_VALID_FIELD, (value) =>
				checkToAllHyphens(value)
			)
			.test('all-hyphens-and-spaces-check', NOT_VALID_FIELD, (value) =>
				checkToAllHyphensAndSpaces(value)
			)
			.test('only-symbols-check', NOT_VALID_FIELD, (value) => {
				if (!isMandatory && !value) {
					return true;
				}

				return !checkIfNoLettersOrNumbers(value);
			})
			.matches(TEXT_TYPE_1_REG_EXP, NOT_VALID_FIELD)
			.min(MIN_CHARACTERS, getMinLengthErrorMessage(MIN_CHARACTERS))
			.max(MAX_CHARACTERS, getMaxLengthErrorMessage(MAX_CHARACTERS));
	};

	const fieldSchema = afterTransform(yup.string());

	if (!isMandatory) {
		return fieldSchema.notRequired();
	}

	return fieldSchema;
};
