import * as yup from 'yup';

import {
	FIELD_IS_REQUIRED,
	getMaxLengthErrorMessage,
	getMinLengthErrorMessage,
} from '@/constants/errors';
import {
	checkEmptyField,
	checkIfNoLettersOrNumbers,
	getNotValidFieldMessage,
	getPlainText,
} from '@/forms/utils';
import { trimmedEmptyValidityTest } from '@/helpers/trimmedEmptyValidityTest';

const MIN_CHARACTERS = 2;

export const DEFAULT_LONG_TEXT_TYPE_1_MAX_CHARACTERS = 1000;

export const longTextType1Schema = (
	fieldName: string,
	isMandatory = false,
	minCountOfCharacters = MIN_CHARACTERS,
	isIncludeSymbolCheck = true,
	isPlateEditor = false,
	maxCountOfCharacters = DEFAULT_LONG_TEXT_TYPE_1_MAX_CHARACTERS
) => {
	const NOT_VALID_FIELD = getNotValidFieldMessage(fieldName);

	const afterTransform = (schema: yup.StringSchema) => {
		const withTrimAndEmptyValidityCheck = schema
			.nullable()
			.transform((value) => {
				if (!isMandatory && (value === '' || value === null)) {
					return null;
				}

				if (isPlateEditor) {
					value = getPlainText(value);
				}

				return String(value).trim();
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
			.test('only-symbols-check', NOT_VALID_FIELD, (value) => {
				if (value?.length && isIncludeSymbolCheck) {
					return !checkIfNoLettersOrNumbers(value);
				}

				return isMandatory ? !checkEmptyField(value) : true;
			})
			.min(minCountOfCharacters, getMinLengthErrorMessage(minCountOfCharacters))
			.max(
				maxCountOfCharacters,
				getMaxLengthErrorMessage(maxCountOfCharacters)
			);
	};

	const fieldSchema = afterTransform(yup.string());

	if (!isMandatory) {
		return fieldSchema.notRequired();
	}

	return fieldSchema;
};
